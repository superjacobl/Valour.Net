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
    }
}
