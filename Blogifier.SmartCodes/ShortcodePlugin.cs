using System.Text;
using System.Text.RegularExpressions;

namespace Framework.Providers.Wiki.Interprete
{
    public abstract class ShortcodePlugin
    {
        public Regex Expression { get; set; }
        public bool ShouldScanForExcludedParts { get; set; }
        public Stack<TextReplacement> Replacements { get; set; }
        private ShortcodeParser _parser = null;


        public ShortcodeParser Parser
        {
            get { return _parser; }
        }

        public ShortcodePlugin SetRegex(Regex regex)
        {
            this.Expression = regex;
            return this;
        }

        public ShortcodePlugin AppendParser(ShortcodeParser parser)
        {
            _parser = parser;
            parser.Plugins.Add(this);
            return this;
        }

        public ShortcodePlugin RecalcExclussionsOnFinish(bool shouldRecalc)
        {
            this.ShouldScanForExcludedParts = true;
            return this;
        }

        public ShortcodePlugin()
        {
            this.Replacements = new Stack<TextReplacement>();
        }

        public virtual void Preprocess(StringBuilder content)
        { }

        public async Task DoProcess(StringBuilder content)
        {
            Preprocess(content);
            await ApplyShortcode(content);
            PostProcess(content);
        }

        public abstract Task ApplyShortcode(StringBuilder cadena);

        public virtual void PostProcess(StringBuilder cadena)
        {
            ApplyTextReplacements();
        }

        public void ApplyTextReplacements()
        {
            while (Replacements.Any())
            {
                var replacement = Replacements.Pop();
                ApplyTextReplacement(replacement);
            }
        }

        private void ApplyTextReplacement(TextReplacement reemplazo)
        {
            Parser.ShortcodeInfo.Content.Remove(reemplazo.TextRange.From, reemplazo.TextRange.Length);
            Parser.ShortcodeInfo.Content.Insert(reemplazo.TextRange.From, reemplazo.Content.ToString());
        }
    }
}
