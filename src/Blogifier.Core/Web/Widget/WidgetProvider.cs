using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blogifier.Core.Web
{
    public class WidgetProvider : IWidgetViewProvider
    {
        private ILookup<string, string> _widgetsByArea;

        public IEnumerable<IWidgetListProvider> WidgetListProvider { get; }

        public WidgetProvider(IEnumerable<IWidgetListProvider> widgetListProvider)
        {
            WidgetListProvider = widgetListProvider;
        }

        public List<WidgetModel> GetWidgetList(string area, object model)
        {
            _widgetsByArea = _widgetsByArea ?? WidgetListProvider
                .SelectMany(x => x.GetPluginWidgets())
                .ToLookup(x => x.areaName, x => x.widgetName);

            if (!_widgetsByArea.Contains(area)) return new List<WidgetModel>();

            return _widgetsByArea[area]
                .Select(widgetName => new WidgetModel(widgetName, area))
                .ToList();
        }
    }
}
