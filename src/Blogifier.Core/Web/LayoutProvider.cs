using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;

using MimeKit;

using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Blogifier.Core.Web
{
    public class LayoutProvider : ILayoutProvider
    {
        public List<string> TopCssList { get; } = new List<string>();
        public List<ScriptReference> BottonScriptList { get; } = new List<ScriptReference>();

        public void AddCss(string cssPath)
        {
            TopCssList.Add(cssPath);
        }

        public void AddScript(string jsPath, bool isAsync = false)
        {
            BottonScriptList.Add(new ScriptReference(jsPath, isAsync));
        }

        public IHtmlContent TopCss()
        {
            var result = new StringBuilder();
            foreach (var item in TopCssList)
            {
                result.AppendLine($@"<link href=""{item}"" rel=""stylesheet"" type=""text/css"" />");
            }
            return new HtmlString(result.ToString());
        }

        public IHtmlContent BottomJs()
        {
            var result = new StringBuilder();
            foreach (var item in BottonScriptList)
            {
                result.Append($@"<script {(item.IsAsync ? "defer" : "")} src=""{item.JsPath}"" /></script>");
            }
            return new HtmlString(result.ToString());
        }
    }
}
