using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Providers.Wiki.Interprete
{
    public class ShortcodeContainer
    {
        public StringBuilder Content { get; set; }
        public List<TextRange> ExcludedAreas { get; set; }
        public ShortcodeContainer()
        {
            Content = new StringBuilder();
            ExcludedAreas = new List<TextRange>();
        }
    }
}
