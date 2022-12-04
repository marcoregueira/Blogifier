using Blogifier.Core.Extensions;
using Blogifier.Core.Providers;

using Microsoft.AspNetCore.Mvc;

namespace Blogifier.Plugin.Theme.Freelancer.Components;

[ViewComponent(Name = "IndexFooter")]
public class IndexFooterComponent : ViewComponent
{
    IPostProvider postProvider;
    IAuthorProvider authorProvider;

    public IndexFooterComponent(IPostProvider postProvider, IAuthorProvider authorProvider)
    {
        this.postProvider = postProvider;
        this.authorProvider = authorProvider;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var column1 = await postProvider.GetPostBySlug("Index_Footer_2Cols");
        if (column1 == null)
            return Content("");
      
        var parts = column1.Content
            .Replace("== COLUMN1", "")
            .Split("== COLUMN2");

        var col1 = parts.ElementAt(0).MdToHtml();
        var col2 = parts.ElementAtOrDefault(1)?.MdToHtml();
        var model = new { column1 = col1, column2 = col2 };
        return View(model);
    }
}
