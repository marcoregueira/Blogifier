using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blogifier.Plugin.Theme.One.Controllers
{
    public class ThemeExtrasController : Controller
    {
        [Route("/theme.one/copyright")]
        public IActionResult ThemeCopyright()
        {
            return View("ThemeCopyright");
        }
    }
}
