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
        private CommandInfo CurrentCommand { get; set; }

        public CommandBuilder()
        {
            CurrentCommand = new CommandInfo();
        }

        public CommandInfo ProcessMethod(MethodInfo Method)
        {
            //DEBUG
            Type magicType = Method.DeclaringType;
            ConstructorInfo magicConstructor = magicType.GetConstructor(Type.EmptyTypes);
            object magicClassObject = magicConstructor.Invoke(Array.Empty<object>());

            
            Method.Invoke(magicClassObject, null);

            return new CommandInfo();
        }
    }
}
