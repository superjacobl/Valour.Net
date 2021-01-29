using System;
using System.Collections.Generic;
using System.Linq;
using Valour.Net.ErrorHandling;

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

        public void AddCommand(CommandInfo command)
        {
            if (Commands.Any(c => c.MainAlias.ToLower() == command.MainAlias) || Commands.Any(c => c.Aliases.Contains(command.MainAlias)))
            {
                throw new GenericError($"Multiple commands may not use the same alias, Conflicting alias : {command.MainAlias.ToLower()}", ErrorSeverity.FATAL);
            }
            if (Commands.Any(c => c.Aliases.Intersect(command.Aliases).Any()))
            {
                throw new GenericError($"Multiple commands may not contain the same alternate aliases, Main Alias of conflicting command : {command.MainAlias.ToLower()}", ErrorSeverity.FATAL);
            }
            Commands.Add(command);
        }
    }
}
