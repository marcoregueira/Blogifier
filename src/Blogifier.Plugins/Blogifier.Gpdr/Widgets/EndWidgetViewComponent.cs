using Microsoft.AspNetCore.Mvc;
using Blogifier.Core.Web;

namespace Blogifier.Plugin.Gpdr.Widgets
{
    [ViewComponent(Name = "GPDR__End")]
    public class EndWidgetViewComponent : ViewComponent
    {
        private ILayoutProvider _layoutProvider;

        public EndWidgetViewComponent(ILayoutProvider layoutProvider)
        {
            _layoutProvider = layoutProvider;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //_layoutProvider.AddScript("/plugins/gpdr/cookieconsent/cookieconsent.js");
            //_layoutProvider.AddCss("/plugins/gpdr/cookieconsent/cookieconsent.css");
            //_layoutProvider.AddScript("/plugins/gpdr/cookieconsent/cookieconsent-init.js");



            return View("Default");
        }
    }
}
