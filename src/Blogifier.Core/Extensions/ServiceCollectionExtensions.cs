using Blogifier.Core.Data;
using Blogifier.Core.Providers;
using Blogifier.Core.Web;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace Blogifier.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlogDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("Blogifier");
            var conn = section.GetValue<string>("ConnString");

            if (section.GetValue<string>("DbProvider") == "SQLite")
            {
                services.AddDbContext<AppDbContext>(o => o.UseSqlite(conn));
            }
            else
            {
                // The standard version of Blogifier allows multiple providers,
                // but requires to change the code to add new migrations.
                // I'm removing all providers that haver not explicit support OTB
                // until migrations for each provider are implemented in the standar build.
                throw new NotImplementedException("Only SQLite is implemented by now.");
            }
            services.AddDatabaseDeveloperPageExceptionFilter();
            return services;
        }

        public static IServiceCollection AddBlogProviders(this IServiceCollection services)
        {
            services.AddScoped<IAuthorProvider, AuthorProvider>();
            services.AddScoped<IBlogProvider, BlogProvider>();
            services.AddScoped<IPostProvider, PostProvider>();
            services.AddScoped<IStorageProvider, StorageProvider>();
            services.AddScoped<IFeedProvider, FeedProvider>();
            services.AddScoped<ICategoryProvider, CategoryProvider>();
            services.AddScoped<IAnalyticsProvider, AnalyticsProvider>();
            services.AddScoped<INewsletterProvider, NewsletterProvider>();
            services.AddScoped<IEmailProvider, MailKitProvider>();
            services.AddScoped<IThemeProvider, ThemeProvider>();
            services.AddScoped<ISyndicationProvider, SyndicationProvider>();
            services.AddScoped<IAboutProvider, AboutProvider>();

            services.AddScoped<IWidgetViewProvider, WidgetProvider>();
            services.AddScoped<ILayoutProvider, LayoutProvider>();


            return services;
        }
    }
}
