namespace Blogifier.Plugin.Theme.Freelancer;

public class FreelancerThemeSettings
{
    /// <summary>
    /// Enables a several theme pages identified as ThemeExtras in the views
    /// i.e: Seed page and Copyright page.
    /// </summary>
    public bool EnableThemeConfigurationPage { get; set; }

    /// <summary>
    /// Will show a link from the porfolio items to the corresponding post elements
    /// enabling navigation
    /// </summary>
    public bool EnableBlogPages { get; set; } = false;
    public string CopyrightLine { get; set; } = "Freelancer Website 2022";

    public bool BlogifierContactEnabled { get; set; } = false;

    /// <summary>
    /// This settings enables the contact section using SB Forms provided by Smart Bootstrap
    /// Set to false to enable the default Blogifiear newsletter service
    /// </summary>
    public bool SmartBootstrapContactEnabled { get; set; } = true;
    public string SmartbootstrapApiKey { get; set; } = "***DUMMY KEY***";

    public string SocialFacebook { get; set; } = "#";
    public string SocialTwitter { get; set; } = "#";
    public string SocialLinkedIn { get; set; } = "#";
    public string SocialDribble { get; set; } = "#";
    public string SocialGithub { get; set; } = "#";

    public bool ShowPostInfo { get; set; } = true;
    public bool ShowPostAuthorFooter {get;set;} = true;
    public string AboutSectionIcon { get; set; } = "fa-download";
    public string AboutSectionCallToAction { get; set; } = "Free download!";
    public string AboutSectionLink { get; set; } = "https://blogifier.net";

}
