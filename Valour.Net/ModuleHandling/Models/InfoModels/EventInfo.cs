using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Valour.Net.Models;

namespace Valour.Net.CommandHandling.InfoModels
{
    public class EventInfo
    {
        public string EventName { get; set; }
        public ModuleInfo moduleInfo {get ;set;}
        public MethodInfo Method { get; set; }

        public EventInfo()
        {

        }
    }
}
