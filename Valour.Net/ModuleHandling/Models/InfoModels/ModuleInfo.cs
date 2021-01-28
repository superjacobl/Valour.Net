using System;
using System.Collections.Generic;

namespace Valour.Net.CommandHandling.InfoModels
{
    public class ModuleInfo
    {
        public string Name { get; set; }
        public List<CommandInfo> Commands { get; set; }
        public List<Attribute> Attributes { get; set; }
        public CommandModuleBase Instance { get; set; }

        public ModuleInfo(string Name, List<CommandInfo> Commands, List<Attribute> Attributes, CommandModuleBase Instance)
        {
            this.Name = Name;
            this.Commands = Commands;
            this.Attributes = Attributes;
            this.Instance = Instance;
        }

        //DEBUG
        public ModuleInfo()
        {

        }
    }
}
