using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling.InfoModels;

namespace Valour.Net.CommandHandling
{
    public static class CommandService
    {
        public static List<ModuleInfo> _Modules { get; set; } = new List<ModuleInfo>();
        public static List<CommandInfo> _Commands { get; set; } = new List<CommandInfo>();


        //private Dictionary<CommandInfo, ModuleInfo> CommandModuleMap { get; set; }

        public static void RegisterCommand(CommandInfo command)
        {
            //Error checking code needed
            _Commands.Add(command);
        }

        public static void RegisterModule(ModuleInfo module)
        {
            //Error checking code needed
            _Modules.Add(module);
        }
        public static async Task<CommandInfo> RunCommandString(string commandname, List<string> args, CommandContext ctx)
        {
            CommandInfo command;
            foreach (ModuleInfo module in _Modules) {
                command = await module.GetCommand(commandname, args, ctx);
                if (command != null) {
                    return command;
                }
            }
            command = _Commands.FirstOrDefault(commandlookup => commandlookup.IsFallback == true && (commandlookup.MainAlias == commandname || commandlookup.Aliases.Contains(commandname)));
            return command;
        }
    }
}
