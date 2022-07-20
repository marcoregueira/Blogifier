using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blogifier.Core.Web
{
    public interface IWidgetListProvider
    {
        List<(string widgetName, string areaName)> GetPluginWidgets();
    }
}
