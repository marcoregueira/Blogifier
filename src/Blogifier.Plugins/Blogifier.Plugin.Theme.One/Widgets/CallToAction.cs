using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Blogifier.Plugin.Theme.One.Widgets
{
    [ViewComponent(Name = "Code__CallToAction")]
    public class CallToAction : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var jsonData = Convert.ToString(ViewComponentContext?.Arguments.First().Value);
            var arguments = JsonSerializer.Deserialize<CallToActionArguments>(jsonData, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            return View("Default", arguments);
        }

        public class CallToActionArguments
        {
            public string Title { get; set; }
            public string Target { get; set; }
            public bool ShowIcon { get; set; } = true;
        }
    }
}
