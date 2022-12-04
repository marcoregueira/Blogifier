
using Microsoft.AspNetCore.Mvc;

namespace Blogifier.Plugin.Theme.Freelancer.Widgets;

[ViewComponent(Name = "Freelancer__SampleWidget")]
public class SampleWidgetComponent : ViewComponent
{
    public SampleWidgetComponent()
    {
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View("Default");
    }
}
