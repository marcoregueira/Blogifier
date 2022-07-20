using Microsoft.AspNetCore.Mvc;

namespace Blogifier.Plugin.Theme.One.Widgets
{
    [ViewComponent(Name = "One__Page_Start")]
    public class HeaderWidgetViewComponent : ViewComponent
    {
        public HeaderWidgetViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //_layoutProvider.AddScript("/plugins/gpdr/cookieconsent/cookieconsent.js");
            //_layoutProvider.AddScript("/plugins/gpdr/cookieconsent/cookieconsent-init.js");
            //_layoutProvider.AddCss("/plugins/gpdr/cookieconsent/cookieconsent.css");
            // return View("~/../../plugins/gpdr/Views/Shared/Components/GPDR_Header/Default.cshtml");
            return View("Default");
        }
    }
}
