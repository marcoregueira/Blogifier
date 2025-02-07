using Blogifier.Core.Extensions;
using Blogifier.Core.Providers;
using Blogifier.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Framework.Providers.Wiki.Interprete;
using Blogifier.SmartCodes.Shared;
using Blogifier.Components;
namespace Blogifier.Controllers;

public class HomeController : Controller
{
    protected readonly IBlogProvider _blogProvider;
    protected readonly IPostProvider _postProvider;
    protected readonly IFeedProvider _feedProvider;
    protected readonly IAuthorProvider _authorProvider;
    protected readonly IThemeProvider _themeProvider;
    protected readonly IStorageProvider _storageProvider;
    protected readonly ICompositeViewEngine _compositeViewEngine;
    protected readonly ShortcodeParser _shortCodeParser;
    private readonly ISmartCodeRenderer _smartCodeRenderer;

    public object renderer { get; private set; }

    public HomeController(IBlogProvider blogProvider,
        IPostProvider postProvider, IFeedProvider feedProvider, IAuthorProvider authorProvider, IThemeProvider themeProvider,
        IStorageProvider storageProvider, ICompositeViewEngine compositeViewEngine, ShortcodeParser wiki,
        ISmartCodeRenderer smartCodeRenderer)
    {
        _blogProvider = blogProvider;
        _postProvider = postProvider;
        _feedProvider = feedProvider;
        _authorProvider = authorProvider;
        _themeProvider = themeProvider;
        _storageProvider = storageProvider;
        _compositeViewEngine = compositeViewEngine;
        _shortCodeParser = wiki;
        this._smartCodeRenderer = smartCodeRenderer;
    }

    public async Task<IActionResult> Index(int page = 1)
    {

        var model = await getBlogPosts(pager: page);

        //If no blogs are setup redirect to first time registration
        if (model == null)
        {
            return Redirect("~/admin/register");
        }

        return View($"Index", model);
    }

    [HttpGet("/{slug}")]
    public async Task<IActionResult> Index(string slug)
    {
        if (!string.IsNullOrEmpty(slug))
        {
            return await getSingleBlogPost(slug);
        }
        return Redirect("~/");
    }

    [HttpGet("/admin")]
    public async Task<IActionResult> Admin()
    {
        return File("~/index.html", "text/html");
    }

    [HttpPost]
    public async Task<IActionResult> Search(string term, int page = 1)
    {

        if (!string.IsNullOrEmpty(term))
        {
            var model = await getBlogPosts(term, page);
            string viewPath = $"Search";
            if (IsViewExists(viewPath))
                return View(viewPath, model);
            else
                return Redirect("~/");
        }
        else
        {
            return Redirect("~/");
        }
    }

    [HttpGet("categories/{category}")]
    public async Task<IActionResult> Categories(string category, int page = 1)
    {
        var model = await getBlogPosts("", page, category);
        string viewPath = "Category";

        ViewBag.Category = category;

        //marco aquí es necesario esto? devolvemos "Category" y ya...
        if (IsViewExists(viewPath))
            return View(viewPath, model);

        return View("Index", model);
    }

    [HttpGet("posts/{slug}")]
    public async Task<IActionResult> Single(string slug)
    {
        return await getSingleBlogPost(slug);
    }

    [HttpGet("error")]
    public async Task<IActionResult> Error()
    {
        try
        {
            PostModel model = new PostModel();
            model.Blog = await _blogProvider.GetBlogItem();
            string viewPath = $"404.cshtml";
            if (IsViewExists(viewPath))
                return View(viewPath, model);
            return View($"~/Views/Error.cshtml");
        }
        catch
        {
            return View($"~/Views/Error.cshtml");
        }
    }

    [ResponseCache(Duration = 1200)]
    [HttpGet("feed/{type}")]
    public async Task<IActionResult> Rss(string type)
    {
        string host = Request.Scheme + "://" + Request.Host;
        var blog = await _blogProvider.GetBlog();

        var posts = await _feedProvider.GetEntries(type, host);
        var items = new List<SyndicationItem>();

        var feed = new SyndicationFeed(
             blog.Title,
             blog.Description,
             new Uri(host),
             host,
             posts.FirstOrDefault().Published
        );

        if (posts != null && posts.Count() > 0)
        {
            foreach (var post in posts)
            {
                var item = new SyndicationItem(
                     post.Title,
                     post.Description.MdToHtml(),
                     new Uri(post.Id),
                     post.Id,
                     post.Published
                );
                item.PublishDate = post.Published;
                items.Add(item);
            }
        }
        feed.Items = items;

        var settings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            NewLineHandling = NewLineHandling.Entitize,
            NewLineOnAttributes = true,
            Indent = true
        };

        using (var stream = new MemoryStream())
        {
            using (var xmlWriter = XmlWriter.Create(stream, settings))
            {
                var rssFormatter = new Rss20FeedFormatter(feed, false);
                rssFormatter.WriteTo(xmlWriter);
                xmlWriter.Flush();
            }
            return File(stream.ToArray(), "application/xml; charset=utf-8");
        }
    }

    private bool IsViewExists(string viewPath)
    {
        var result = _compositeViewEngine.GetView("", viewPath, false);
        return result.Success;
    }

    [NonAction]
    public async Task<IActionResult> getSingleBlogPost(string slug)
    {
        try
        {
            ViewBag.Slug = slug;
            var model = await _postProvider.GetPostModel(slug);

            // If not found redirect to create post.
            if (model == null && User.Identity.IsAuthenticated)
            {
                return Redirect("~/admin/editor/" + slug);
            }

            if (model == null)
            {
                return Redirect("~/error");
            }
            // If unpublished and unauthorised redirect to error / 404.
            if (model.Post.Published == DateTime.MinValue && !User.Identity.IsAuthenticated)
            {
                return Redirect("~/error");
            }

            model.Blog = await _blogProvider.GetBlogItem();
            model.Post.Description = model.Post.Description.MdToHtml();
            model.Post.Content = model.Post.Content.MdToHtml();
            model.Post.Content = await ParseShortcodesAsync(model.Post.Content, ControllerContext, ViewData, TempData);

            if (!model.Post.Author.Avatar.StartsWith("data:"))
                model.Post.Author.Avatar = Url.Content($"~/{model.Post.Author.Avatar}");

            if (model.Post.PostType == PostType.Page)
            {
                string viewPath = "Page";
                if (IsViewExists(viewPath))
                    return View(viewPath, model);
            }

            return View($"Post", model);
        }
        catch
        {
            return Redirect("~/error");
        }
    }
    public async Task<ListModel> getBlogPosts(string term = "", int pager = 1, string category = "", string slug = "")
    {

        var model = new ListModel { };

        try
        {
            model.Blog = await _blogProvider.GetBlogItem();
        }
        catch
        {
            return null;
        }

        model.Pager = new Pager(pager, model.Blog.ItemsPerPage);

        if (!string.IsNullOrEmpty(category))
        {
            model.PostListType = PostListType.Category;
            model.Posts = await _postProvider.GetList(model.Pager, 0, category, "PF");
        }
        else if (string.IsNullOrEmpty(term))
        {
            model.PostListType = PostListType.Blog;
            if (model.Blog.IncludeFeatured)
                model.Posts = await _postProvider.GetList(model.Pager, 0, "", "FP");
            else
                model.Posts = await _postProvider.GetList(model.Pager, 0, "", "P");
        }
        else
        {
            model.PostListType = PostListType.Search;
            model.Blog.Title = term;
            model.Blog.Description = "";
            model.Posts = await _postProvider.Search(model.Pager, term, 0, "FP");
        }

        if (model.Pager.ShowOlder) model.Pager.LinkToOlder = $"?page={model.Pager.Older}";
        if (model.Pager.ShowNewer) model.Pager.LinkToNewer = $"?page={model.Pager.Newer}";

        return model;
    }

    private async Task<string> ParseShortcodesAsync(string text, ControllerContext controllerContext, ViewDataDictionary viewData, ITempDataDictionary tempData)
    {
        var renderer = _smartCodeRenderer as SmartCodeRenderer;
        renderer.SetContext(controllerContext, viewData, tempData);
        _shortCodeParser.SetContent(text);
        await _shortCodeParser.ApplyShortcodes();
        return _shortCodeParser.ShortcodeInfo.Content.ToString();
    }
}
