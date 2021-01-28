using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling.InfoModels;

namespace Valour.Net.CommandHandling
{
    public class CommandService
    {
        private List<ModuleInfo> _Modules { get; set; }
        private List<CommandInfo> _Commands { get; set; }


        //private Dictionary<CommandInfo, ModuleInfo> CommandModuleMap { get; set; }

        public CommandService()
        {
            _Modules = new List<ModuleInfo>();
            _Commands = new List<CommandInfo>();
        }

        public void RegisterCommand(CommandInfo command)
        {
            //Error checking code needed
            _Commands.Add(command);
        }

        public void RegisterModule(ModuleInfo module)
        {
            //Error checking code needed
            _Modules.Add(module);
        }
    }
}
