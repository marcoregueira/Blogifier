using Blogifier.Core.Providers;
using Blogifier.Shared;

namespace Blogifier.Plugin.Theme.Freelancer.Seed;

public class SeedTool
{
    private readonly IAuthorProvider authorProvider;
    private readonly IPostProvider postProvider;
    private readonly ICategoryProvider categoryProvider;

    public SeedTool(IAuthorProvider authorProvider,
        IPostProvider postProvider,
        ICategoryProvider categoryProvider)
    {
        this.authorProvider = authorProvider;
        this.postProvider = postProvider;
        this.categoryProvider = categoryProvider;
    }

    public async Task<Post> SeedHomeAboutSection()
    {
        const string aboutSectionPostId = "Index_Part_2Cols";
        var author = (await authorProvider.GetAuthors()).First();

        var post = await postProvider.GetPostBySlug(aboutSectionPostId);

        if (post != null)
            return post;

        var aboutSection = new Post()
        {
            AuthorId = author.Id,
            Published = DateTime.MinValue,
            Slug = aboutSectionPostId,
            Title = "About",
            PostType = PostType.Page,
            Description = "Home page About section. It supports two columns separated by a \"== COLUMN 2\" title. Keep unpublished and do not change the slug.",
            Content = @"== COLUMN1
Freelancer is a free bootstrap theme created by Start Bootstrap. The download includes the complete source files including HTML, CSS, and JavaScript as well as optional SASS stylesheets for easy customization.

== COLUMN2
You can create your own custom avatar for the masthead, change the icon in the dividers, and add your email address to the contact form to make it fully functional!"
        };

        await postProvider.Add(aboutSection);
        return aboutSection;
    }

    public async Task<Post> SeedHomeFooterSection()
    {
        const string footerSectionPostId = "Index_Footer_2Cols";
        var author = (await authorProvider.GetAuthors()).First();

        var post = await postProvider.GetPostBySlug(footerSectionPostId);

        if (post != null)
            return post;

        var aboutSection = new Post()
        {
            AuthorId = author.Id,
            Published = DateTime.MinValue,
            Slug = "Index_Footer_2Cols",
            Title = "About",
            PostType = PostType.Page,
            Description = "Footer section of the page. It supports two columns separated by a \"== COLUMN 2\" title. Keep unpublished and do not change the slug.",
            Content = @"== COLUMN1
#### LOCATION

2215 John Daniel Drive

Clark, MO 65243

== COLUMN2
#### ABOUT FREELANCER

Freelance is a free to use, MIT licensed Bootstrap theme created by [Start Bootstrap](https://startbootstrap.com/theme/freelancer) and [adapted for Blogifier](https://github.com/marcoregueira/Blogifier)."
        };

        await postProvider.Add(aboutSection);
        return aboutSection;
    }

    public async Task SeedPortfolioCategoryAsync()
    {
        var category = await categoryProvider.SaveCategory("Portfolio");
        var baseFolder = "_content/Blogifier.Plugin.Theme.Freelancer/assets/img/portfolio/";
        var body = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Mollitia neque assumenda ipsam nihil, molestias magnam, recusandae quos quis inventore quisquam velit asperiores, vitae? Reprehenderit soluta, eos quod consequuntur itaque. Nam.";
        var author = (await authorProvider.GetAuthors()).First();

        var postData = new[] {
            new[]{ "cabin.png", "Log Cabin"},
            new[]{ "cake.png", "Tasty Cake"},
            new[]{ "circus.png", "Circus Tent"},
            new[]{ "game.png", "Controller"},
            new[]{ "safe.png", "Locked Safe"},
            new[]{ "submarine.png", "Submarine"}
        };

        foreach (var postItem in postData)
        {
            var post = new Post()
            {
                AuthorId = author.Id,
                Cover = baseFolder + postItem[0],
                Content = body,
                PostType = PostType.Post,
                Title = postItem[1],
                Published = DateTime.Today,
                Slug = postItem[1].Replace(" ", "_"),
                Description = body,
                IsFeatured = true
            };
            await postProvider.Add(post);
            await categoryProvider.AddPostCategory(post.Id, "Portfolio");
        }
    }
}
