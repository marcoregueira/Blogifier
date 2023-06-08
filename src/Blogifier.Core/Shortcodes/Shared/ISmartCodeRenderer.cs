using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blogifier.SmartCodes.Shared
{
    public interface ISmartCodeRenderer
    {
        Task<string> Render(string componentName, string paramValues);
    }
}
