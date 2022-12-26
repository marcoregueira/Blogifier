using Framework.Providers.Wiki.Interprete;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System;
using Blogifier.SmartCodes.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Ocsp;

namespace Blogifier.Components;

public class SmartCodeRenderer : ISmartCodeRenderer
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<SmartCodeRenderer> logger;
    private ViewDataDictionary _viewData;
    private ITempDataDictionary _tempData;
    private ControllerContext _controllerContext;

    public SmartCodeRenderer(IServiceProvider serviceProvider, ILogger<SmartCodeRenderer> logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    public async Task<string> Render(string componentName, string paramValues)
    {
        var helper = new DefaultViewComponentHelper(
            serviceProvider.GetRequiredService<IViewComponentDescriptorCollectionProvider>(),
            HtmlEncoder.Default,
            serviceProvider.GetRequiredService<IViewComponentSelector>(),
            serviceProvider.GetRequiredService<IViewComponentInvokerFactory>(),
            serviceProvider.GetRequiredService<IViewBufferScope>());

        try
        {
            using (var writer = new StringWriter())
            {
                var context = new ViewContext(_controllerContext, NullViewAlternate.Instance, _viewData, _tempData, writer, new HtmlHelperOptions());
                helper.Contextualize(context);
                var result = await helper.InvokeAsync(componentName, new { JsonData = paramValues });
                result.WriteTo(writer, HtmlEncoder.Default);
                await writer.FlushAsync();
                return writer.ToString();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return "";
        }
    }

    internal void SetContext(ControllerContext controllerContext, ViewDataDictionary viewData, ITempDataDictionary tempData)
    {
        _controllerContext = controllerContext;
        _viewData = viewData;
        _tempData = tempData;
    }
}
