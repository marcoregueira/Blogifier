using System.Collections.Generic;

namespace Blogifier.Core.Web
{
    public interface IWidgetViewProvider
    {
        List<WidgetModel> GetWidgetList(string area, object model);

    }
}
