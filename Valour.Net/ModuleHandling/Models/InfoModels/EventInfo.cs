using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Valour.Net.ModuleHandling.Models.InfoModels;

namespace Valour.Net.CommandHandling.InfoModels
{
    internal class EventInfo
    {
        public EventType eventType { get; set; }
        public ModuleInfo moduleInfo {get ;set;}
        public ValourMethodInfo Method { get; set; }

        public EventInfo()
        {

        }
    }

    
}
