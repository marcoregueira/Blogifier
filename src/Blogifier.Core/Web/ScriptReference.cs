namespace Blogifier.Core.Web
{
    public class ScriptReference
    {
        public string JsPath { get; }
        public bool IsAsync { get; set; }

        public ScriptReference(string jsPath, bool isAsync)
        {
            JsPath = jsPath;
            IsAsync = isAsync;
        }
    }
}
