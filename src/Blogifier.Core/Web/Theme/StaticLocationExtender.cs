using Microsoft.AspNetCore.Mvc.Razor;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blogifier.Core.Web.Theme
{
    public class StaticLocationExtender : IViewLocationExpander
    {
        private readonly ILayoutProvider layout;
        private readonly string baseDirectory;

        public StaticLocationExtender(string baseDirectory)
        {
            this.baseDirectory = baseDirectory;
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            // https://github.com/maxtoroq/MvcCodeRouting/blob/master/docs/Embedded-Views.md
            var layout = context.ActionContext.HttpContext.RequestServices.GetService(typeof(ILayoutProvider));

            //get theme name
            //get theme locations

            if (context.AreaName != "")
                Console.WriteLine("area" + context.AreaName);

            var locations = viewLocations
                .Select(x => x.Replace("Views/", baseDirectory + "/"))
                .Concat(viewLocations);

            context.Values["THEME"] = "";
            return locations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {

        }
    }
}
