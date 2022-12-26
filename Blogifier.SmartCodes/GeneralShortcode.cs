using Blogifier.SmartCodes.Shared;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Framework.Providers.Wiki.Interprete;

public class GeneralShortcode : ShortcodePlugin
{
    static Regex m_ExpresionRegular = new Regex(@"(\[\[.+?\]\])|(\[.+?\])", RegexOptions.Compiled);
    private readonly IServiceProvider serviceProvider;

    public GeneralShortcode(IServiceProvider serviceProvider)
        : base()
    {
        this.Expression = m_ExpresionRegular;
        this.RecalcExclussionsOnFinish(true);
        this.serviceProvider = serviceProvider;
    }


    public override async Task ApplyShortcode(StringBuilder cadena)
    {
        Match match = this.Expression.Match(Parser.ShortcodeInfo.Content.ToString());
        while (match.Success)
        {
            int finExclusion = 0;
            if (!this.Parser.IsExcludedPosition(match.Index, out finExclusion))
            {
                var replacedText = match.Value;
                System.Diagnostics.Debug.WriteLine(replacedText);

                TextReplacement replacement = new TextReplacement(match.Index, match.Length);
                replacement.Content = new StringBuilder();
                this.Replacements.Push(replacement);
                bool isEmptyTag = (match.Value.Equals("[]") || match.Value.Equals("[[]]"));
                if (!isEmptyTag)
                {
                    var shortcodeText = match.Value.Substring(1, match.Length - 2).Trim();
                    shortcodeText = HttpUtility.HtmlDecode(shortcodeText);
                    var parts = shortcodeText.Split(' ', 2);
                    var shortcodeName = parts[0];
                    var shortcodeOptions = "{}";
                    if (parts.Length > 1)
                    {
                        //We will use Newtonsoft as a simple option string parser
                        //we wont use System.Text.Json because it requires quotes for properties,
                        //while Newtonsoft is more tolerant with non conformant json.
                        var json = $"{{{parts[1]}}}";
                        var options = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json));
                        shortcodeOptions = string.IsNullOrEmpty(options) ? "{}" : options;
                    }
                    replacement.Content.Append(await RenderViewComponentAsync("Code__" + shortcodeName, shortcodeOptions));
                }
            }
            match = match.NextMatch();
        }
    }

    private async Task<string> RenderViewComponentAsync(string v, dynamic options)
    {
        var renderer = serviceProvider.GetService(typeof(ISmartCodeRenderer)) as ISmartCodeRenderer;
        var render = await renderer.Render(v, options);
        return render;
    }
}
