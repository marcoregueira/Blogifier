using Blogifier.Core.Providers;
using Blogifier.Plugin.Theme.Freelancer.Seed;
using Blogifier.Shared;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Blogifier.Plugin.Theme.Freelancer.Controllers;

public class SeedController : Controller
{
    private readonly IBlogProvider blogProvider;
    private readonly IAuthorProvider _authorProvider;
    private readonly IOptions<FreelancerThemeSettings> settings;
    private readonly SeedTool seedTool;

    public SeedController(IBlogProvider blogProvider,
        IAuthorProvider authorProvider,
        IOptions<FreelancerThemeSettings> settings,
        SeedTool seed)
    {
        this.blogProvider = blogProvider;
        _authorProvider = authorProvider;
        this.settings = settings;
        seedTool = seed;
    }

    [HttpGet("/theme.freelancer/seed/")]
    public async Task<IActionResult> SeedAsync()
    {
        if (!settings.Value.EnableThemeConfigurationPage)
            return NotFound();

        var blog = await blogProvider.GetBlog();

        return View("ThemeExtras/Seed", new { Blog = blog });
    }


    [HttpPost("/theme.freelancer/seed/")]
    public async Task<IActionResult> SeedAsync([FromForm] LoginModel model)
    {
        if (!settings.Value.EnableThemeConfigurationPage)
            return NotFound();

        var blog = await blogProvider.GetBlog();
        if (!blog.IncludeFeatured)
        {
            blog.IncludeFeatured = true;
            await blogProvider.Update(blog);
        }

        if (await _authorProvider.Verify(model) == false)
        {
            ViewBag.Error = "You failed to provide a valid user and password";
            //ViewData["error"] = "You failed to provide a valid user and password";
            return View("ThemeExtras/Seed", new { Blog = blog });
        }


        await seedTool.SeedPortfolioCategoryAsync();
        await seedTool.SeedHomeAboutSection();
        await seedTool.SeedHomeFooterSection();

        return Redirect("/");
    }
}
