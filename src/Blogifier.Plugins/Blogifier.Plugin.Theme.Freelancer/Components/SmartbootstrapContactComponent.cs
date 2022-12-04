using Blogifier.Core.Providers;
using Blogifier.Core.Web;
using Blogifier.Plugin.Theme.Freelancer;
using Blogifier.Shared;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blogifier.Plugin.Theme.StartBootstrap.Components
{
    [ViewComponent(Name = "SmartbootstrapContact")]
    public class SmartbootstrapContactComponent : ViewComponent
    {
        // This component implements the SmartBootstrap contact section
        //<!-- This form is pre-integrated with SB Forms.-->
        //<!-- To make this form functional, sign up at-->
        //<!-- https://startbootstrap.com/solution/contact-forms-->
        //<!-- to get an API token!-->

        private readonly ILayoutProvider layout;
        private readonly IOptions<FreelancerThemeSettings> themeSettings;

        public SmartbootstrapContactComponent(ILayoutProvider layout, IOptions<FreelancerThemeSettings> themeSettings)
        {
            this.layout = layout;
            this.themeSettings = themeSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (themeSettings.Value.SmartBootstrapContactEnabled)
            {
                return View();
            }

            return Content("");
        }
    }
}
