using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.Global;

namespace Valour.Net.CommandHandling
{
    /// <summary>
    /// This module base is loaded by the command service but is not checked for registrable commands
    /// </summary>
    public abstract class CommandModuleBase : IModuleBase
    {
    }
}
