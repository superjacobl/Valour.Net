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
        public string GroupName = "";

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
            Commands = new List<CommandInfo>();
        }

        public void AddCommand(CommandInfo command)
        {
            Commands.Add(command);
        }

        public CommandInfo GetCommand(string commandname, List<string> args, CommandContext ctx)
        {

            // check if this a group module

            if (GroupName != "") {
                if (args.Count() > 0) {

                    // args[0] should be the command prefix

                    if (GroupName == commandname) {
                        string subcommandname = args[0];
                        args.RemoveAt(0);
                        foreach (CommandInfo command in Commands) {
                            if (command.CheckIfCommand(subcommandname, args, ctx)) {
                                return command;
                            }
                        }
                    }
                } 
                else {
                    if (GroupName == commandname) {
                        string subcommandname = "";
                        foreach (CommandInfo command in Commands) {
                            if (command.CheckIfCommand(subcommandname, args, ctx)) {
                                return command;
                            }
                        }
                    }
                }
                return null;
            }

            foreach (CommandInfo command in Commands) {
                if (command.CheckIfCommand(commandname, args, ctx)) {
                    return command;
                }
            }
            return null;
        }

    }
}
