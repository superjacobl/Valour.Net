using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling.InfoModels;

namespace Valour.Net.CommandHandling.Builders
{

    
    class CommandBuilder
    {
        private CommandInfo CurrentCommand { get; set; }

        public CommandBuilder()
        {
            CurrentCommand = new CommandInfo();
        }

        public void ProcessMethod(MethodInfo Method)
        {
        
        }
    }
}
