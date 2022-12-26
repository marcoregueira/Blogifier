using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace Blogifier.Components;

public class NullViewAlternate : IView
{
    public static readonly NullViewAlternate Instance = new NullViewAlternate();

    public string Path => string.Empty;

    public Task RenderAsync(ViewContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        return Task.CompletedTask;
    }
}
