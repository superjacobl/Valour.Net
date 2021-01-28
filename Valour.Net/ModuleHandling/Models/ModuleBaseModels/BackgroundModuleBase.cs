using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.Global;

namespace Valour.Net.ModuleHandling
{
    /// <summary>
    /// This module base is loaded by the command service and is not checked for registrable commands
    /// </summary>
    public abstract class BackgroundModuleBase : IModuleBase
    {
    }
}
