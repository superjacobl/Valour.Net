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

namespace Valour.Net.CommandHandling
{
    public class ModuleRegistrar
    {
        public ModuleRegistrar()
        {
            
        }

        /// MOST OF THIS METHOD IS DEBUG
        /// <summary>
        /// Finds all classes that inherit from CommandModuleBase and then finds all methods which have a Command attribute
        /// </summary>
        public static void RegisterAllCommands(ErrorHandler errorHandler)
        {
            errorHandler.ReportError(new GenericError("", ErrorSeverity.FATAL));




            IEnumerable<Type> moudules = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(IModuleBase).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

        
            ///Gets all command modules and processes them
            foreach (Type commandModule in moudules.Where(x => x.BaseType == typeof(CommandModuleBase)))
            {
                ModuleBuilder builder = new();
                GroupAttribute GroupAttr = (GroupAttribute)commandModule.GetCustomAttribute(typeof(GroupAttribute));
                builder.BuildModule(commandModule);
                if (GroupAttr != null) {
                    builder.Module.GroupName = GroupAttr.Prefix;
                }
                CommandService.RegisterModule(builder.Module);
                builder.Register();

                /*
                ConstructorInfo a = commandModule.GetConstructor(Type.EmptyTypes);
                CommandModuleBase classs = (CommandModuleBase)a.Invoke(Array.Empty<object>());
                //Console.WriteLine(b);
                foreach (var method in classs.GetType().GetMethods())
                {
                    Console.WriteLine(method);

                }
                */
            }

            /*



            MethodInfo[] methods = assembly.GetTypes()
                    .SelectMany(t => t.GetMethods())
                    .Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0)
                    .ToArray();

                    //.Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0)
            foreach (MethodInfo method in methods)
            {
                //aa.ProcessMethod(method);

                CommandAttribute myAttributes = (CommandAttribute)Attribute.GetCustomAttribute(method,typeof(CommandAttribute));
                Console.WriteLine($"Method {method.Name} has a command attribute with the text {myAttributes.Name}"); //DEBUG
                foreach (var param in method.GetParameters())
                {
                    Console.WriteLine($"Method {method.Name} has a parameter {param.Name} with type {param.ParameterType}");
                }
                
            }
            */
        }
    }
}
