using Blogifier.Core.Providers;
using Blogifier.Plugin.Theme.Freelancer;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Blogifier.Plugin.Theme.Freelancer.Controllers;

public class ThemeExtrasController : Controller
{
    private readonly IBlogProvider blogProvider;
    private readonly IOptions<FreelancerThemeSettings> settings;

    public ThemeExtrasController(IBlogProvider blogProvider,
        IOptions<FreelancerThemeSettings> settings)
    {
        this.blogProvider = blogProvider;
        this.settings = settings;
    }

    [HttpGet("/theme.freelancer/copyright")]
    public async Task<IActionResult> ThemeCopyrightAsync()
    {
        if (!settings.Value.EnableThemeConfigurationPage)
            return NotFound();

        var blog = await blogProvider.GetBlog();
        return View("ThemeExtras/Copyright", new { Blog = blog });
    }
}
