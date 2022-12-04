using Blogifier.Core.Extensions;
using Blogifier.Core.Providers;

using Microsoft.AspNetCore.Mvc;

namespace Blogifier.Plugin.Theme.Freelancer.Components;

[ViewComponent(Name = "IndexAbout")]
public class IndexAboutComponent : ViewComponent
{
    IPostProvider postProvider;
    IAuthorProvider authorProvider;

    public IndexAboutComponent(IPostProvider postProvider, IAuthorProvider authorProvider)
    {
        this.postProvider = postProvider;
        this.authorProvider = authorProvider;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var post = await postProvider.GetPostBySlug("Index_Part_2Cols");
        if (post == null)
            return Content("");

        var parts = post.Content
            .Replace("== COLUMN1", "")
            .Split("== COLUMN2");
        var col1 = parts.ElementAt(0).MdToHtml();
        var col2 = parts.ElementAtOrDefault(1)?.MdToHtml();
        var model = new { column1 = col1, column2 = col2, title = post.Title };
        return View(model);
    }
}
