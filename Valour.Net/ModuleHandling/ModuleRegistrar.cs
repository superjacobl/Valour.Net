using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling.Builders;
using Valour.Net.ErrorHandling;
using Valour.Net.Global;
using Valour.Net.ModuleHandling;
using Valour.Net.CommandHandling.Attributes;
using System.Diagnostics.Contracts;

namespace Valour.Net.CommandHandling
{
    public class ModuleRegistrar
    {
        public ModuleRegistrar()
        {
            
        }

        /// <summary>
        /// Finds all classes that inherit from CommandModuleBase and then finds all methods which have a Command attribute
        /// </summary>
        public static void RegisterAllCommands()
        {
            IEnumerable<Type> moudules = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(IModuleBase).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

        
            ///Gets all command modules and processes them
            foreach (Type commandModule in moudules.Where(x => x.BaseType == typeof(CommandModuleBase)))
            {
                ModuleBuilder builder = new();
                //Check for command group attribute
                GroupAttribute GroupAttr = (GroupAttribute)commandModule.GetCustomAttribute(typeof(GroupAttribute));
                if (GroupAttr != null) {
                    builder.Module.GroupName = GroupAttr.Prefix;
                }

                builder.BuildModule(commandModule);

                DontAutoLoadAttribute autoLoadAttribute = (DontAutoLoadAttribute)commandModule.GetCustomAttribute(typeof(DontAutoLoadAttribute));               
                if (autoLoadAttribute != null)
                {
                    //Skip module as it is marked to not be loaded at start
                    continue;
                }

                CommandService.RegisterModule(builder.Module);

            }

        }
    }
}
