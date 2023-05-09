using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CustomAttributes;
using System.Reflection;

namespace Valour.Net.ModuleHandling.Models.InfoModels;

internal class ValourMethodInfo
{
    internal List<IEventFilterAttribute> eventFilterAttributes { get; set; }
    public MethodInfo methodInfo { get; set; }

    public ValourMethodInfo (MethodInfo _methodInfo)
    {
        methodInfo = _methodInfo;
        eventFilterAttributes = new();
    }
}
