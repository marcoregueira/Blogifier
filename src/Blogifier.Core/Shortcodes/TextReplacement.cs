using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Providers.Wiki.Interprete
{
    public class TextReplacement : IComparable
    {
        public TextRange TextRange { get; set; }
        public StringBuilder Content { get; set; }
        public TextReplacement()
        {
            this.TextRange = new TextRange();
            this.Content = new StringBuilder();
        }

        public TextReplacement(int startPosition, int lenght)
        {
            this.TextRange = new TextRange(startPosition, lenght);
            this.Content = new StringBuilder();
        }

        public override string ToString()
        {
            return $"{TextRange}: {Content}";
        }

        public int CompareTo(object obj)
        {
            return this.TextRange.From.CompareTo((obj as TextRange).From);
        }
    }
}
