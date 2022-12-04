using Blogifier.Core.Providers;
using Blogifier.Core.Web;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Blogifier.Plugin.Theme.Freelancer.Components;

[ViewComponent(Name = "BlogifierContact")]
public class BlogifierContactComponent : ViewComponent
{
    private readonly ILayoutProvider layout;
    private readonly IOptions<FreelancerThemeSettings> themeSettings;

    public BlogifierContactComponent(ILayoutProvider layout, IOptions<FreelancerThemeSettings> themeSettings)
    {
        this.layout = layout;
        this.themeSettings = themeSettings;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (themeSettings.Value.BlogifierContactEnabled)
        {
            return View();
        }

        return Content("");
    }
}
