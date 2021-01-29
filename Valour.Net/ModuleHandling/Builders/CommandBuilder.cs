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

    
    public class CommandBuilder
    {
        private CommandInfo Command { get; set; }

        public CommandBuilder()
        {
            Command = new CommandInfo();
        }

        public void BuildCommand(MethodInfo Method)
        {
            //DEBUG
            Type magicType = Method.DeclaringType;
            Command.MainAlias = Method.Name;
            Command.Method = Method;
            CommandAttribute CommandAttr = (CommandAttribute)Method.GetCustomAttribute(typeof(CommandAttribute));
            //CommandAttr.
            //Method.Invoke(magicClassObject, null);
            
        }
        


    }
}
