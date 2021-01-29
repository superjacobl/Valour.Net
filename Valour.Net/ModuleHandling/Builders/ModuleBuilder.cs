using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling.InfoModels;

namespace Valour.Net.CommandHandling.Builders
{
    public class ModuleBuilder
    { 
        ModuleInfo Module { get; set; }
        public ModuleBuilder()
        {
            Module = new();
        }

        public void BuildModule(Type module)
        {
            ConstructorInfo constructor = module.GetConstructor(Type.EmptyTypes);
            CommandModuleBase moduleInstance  = (CommandModuleBase)constructor.Invoke(Array.Empty<object>());
            Module.Instance = moduleInstance;
            Module.Name = module.Name;
            

            //Console.WriteLine(b);
            /*
            foreach (var method in module.GetMethods())
            {
                Console.WriteLine(method);

            }
            */
        }

        public void Register()
        {

        }
    }
}
