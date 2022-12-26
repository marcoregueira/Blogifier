using System.Text;
using System.Text.RegularExpressions;

namespace Framework.Providers.Wiki.Interprete;

public class ShortcodeParser
{

    public ShortcodeContainer ShortcodeInfo { get; set; }
    public List<ShortcodePlugin> Plugins { get; set; }
    public IServiceProvider ServiceProvider { get; }

    private static Regex _excludedAreas = new Regex(@"\<nowiki\>(.|\n|\r)+?\<\/nowiki\>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public void ScanForExcludedAreas()
    {
        string text = ShortcodeInfo.Content.ToString();
        Match match = _excludedAreas.Match(text);
        ShortcodeInfo.ExcludedAreas.Clear();
        while (match.Success)
        {
            ShortcodeInfo.ExcludedAreas.Add(new TextRange() { From = match.Index, Length = match.Length });
            match = _excludedAreas.Match(text, match.Index + match.Length);
        }
    }

    public ShortcodeParser(IServiceProvider serviceProvider)
    {
        Plugins = new List<ShortcodePlugin>();
        AppendPlugin(new GeneralShortcode(serviceProvider));
        ServiceProvider = serviceProvider;
    }

    public void SetContent(string content)
    {
        ShortcodeInfo = new ShortcodeContainer();
        ShortcodeInfo.Content = new StringBuilder(content);
    }

    public ShortcodeParser()
    {
        this.Plugins = new List<ShortcodePlugin>();
    }

    public ShortcodeParser AppendPlugin(ShortcodePlugin plugin)
    {
        plugin.AppendParser(this);
        return this;
    }

    public async Task ProcesarAsync()
    {
        foreach (var plugin in this.Plugins)
        {
            ScanForExcludedAreas();
            await plugin.DoProcess(ShortcodeInfo.Content);
            if (plugin.ShouldScanForExcludedParts)
            {
                this.ScanForExcludedAreas();
            }
        }
    }

    public bool IsExcludedPosition(int posicion)
    {
        bool resultado = IsExcludedPosition(posicion, out _);
        return resultado;
    }

    public bool IsExcludedPosition(int position, out int endPosition)
    {
        endPosition = 0;
        foreach (var textRange in ShortcodeInfo.ExcludedAreas)
        {
            if (textRange.From <= position && textRange.To >= position)
            {
                endPosition = textRange.To;
                return true;
            }
            else if (textRange.From > position)
            {
                return false;
            }
        }

        return false;
    }
}
