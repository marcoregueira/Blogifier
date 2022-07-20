using Microsoft.AspNetCore.Mvc;

using System.Linq;
using System.Threading.Tasks;

namespace Blogifier.Core.Web.Widget
{
    [ViewComponent(Name = "Widget")]
    public class Widget : ViewComponent
    {
        private readonly IWidgetViewProvider provider;

        public Widget(IWidgetViewProvider provider)
        {
            this.provider = provider;
        }

        public Task<IViewComponentResult> InvokeAsync(string area, object model)
        {
            var widgetList = provider.GetWidgetList(area, model);
            return Task.FromResult<IViewComponentResult>(widgetList.Any() ? View(widgetList) : Content(""));
        }
    }
}
