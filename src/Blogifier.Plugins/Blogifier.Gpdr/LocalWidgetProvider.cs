using Blogifier.Core.Web;
using Blogifier.Core.Web.Widget;

namespace Blogifier.Plugin.Gpdr
{
    public class LocalWidgetProvider : IWidgetListProvider
    {
        public List<(string widgetName, string areaName)> GetPluginWidgets()
        {
            return new List<(string widgetName, string areaName)>() {
                ("GPDR__Header", WidgetAreas.HEAD_END),
                ("GPDR__End", WidgetAreas.PAGE_END)
            };
        }
    }
}
