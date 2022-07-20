using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.Collections.Generic;

namespace Blogifier.Core.Web
{
    public interface ILayoutProvider
    {
        List<string> TopCssList { get; }
        List<ScriptReference> BottonScriptList { get; }

        void AddCss(string cssPath);
        void AddScript(string jsPath, bool isAsync = false);

        IHtmlContent TopCss();

        IHtmlContent BottomJs();
    }
}
