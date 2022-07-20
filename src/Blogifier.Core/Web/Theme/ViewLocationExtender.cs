using Microsoft.AspNetCore.Mvc.Razor;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Blogifier.Core.Web.Theme
{
    public class ViewLocationExtender : IViewLocationExpander
    {
        private readonly ILayoutProvider layout;

        public ViewLocationExtender()
        {
            //this.layout = layout;



            //
            //context.ActionContext.HttpContext.GetTenant<AppTenant>()?.Theme;
            //context.Values[THEME_KEY] = EngineContext.Current.Resolve<IThemeContext>().GetWorkingThemeNameAsync().Result;
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            // https://github.com/maxtoroq/MvcCodeRouting/blob/master/docs/Embedded-Views.md
            var layout = context.ActionContext.HttpContext.RequestServices.GetService(typeof(ILayoutProvider));

            //get theme name
            //get theme locations

            if (context.AreaName != "")
                Console.WriteLine("area" + context.AreaName);

            //var theme = layout.theme
            // var locations =
            //     new[] {
            //             $"/Views/{theme}/{{1}}/{{0}}.cshtml",
            //             $"/Views/{theme}/Shared/{{0}}.cshtml",
            //     }
            // .Concat(viewLocations);
            var locations = viewLocations
                .Where(x => x.StartsWith("/Views/"))
                .Select(x => x.Replace("/Views/", "/Views/Themes/standard/"))
                .Select(x => x.Replace("/Shared/", "/"));


            context.Values["THEME"] = "";
            return viewLocations.Concat(locations);
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {

        }
    }
}
