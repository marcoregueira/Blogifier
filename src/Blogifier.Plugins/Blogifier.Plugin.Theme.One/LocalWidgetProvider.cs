using Blogifier.Core.Web;
using Blogifier.Core.Web.Widget;

namespace Blogifier.Plugin.Theme.One
{
    public class LocalWidgetProvider : IWidgetListProvider
    {
        public List<(string widgetName, string areaName)> GetPluginWidgets()
        {
            // THIS SECTION DECLARES THE LIST OF WIDGETS IMPLEMENTED BY THIS THEME
            // 
            return new List<(string widgetName, string areaName)>() {
                ("One__Page_Start", WidgetAreas.HEAD_END) };
        }
    }
}
