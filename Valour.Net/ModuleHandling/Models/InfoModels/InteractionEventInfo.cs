﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling.InfoModels;

namespace Valour.Net.ModuleHandling.Models.InfoModels
{
    internal class InteractionEventInfo
    {
        public EmbedIteractionEventType EventType { get; set; }
        public string InteractionFormId { get; set; }
        public string InteractionElementId { get; set; }

        public ModuleInfo moduleInfo { get; set; }
        public MethodInfo Method { get; set; }

        public InteractionEventInfo()
        {

        }
    }
}
