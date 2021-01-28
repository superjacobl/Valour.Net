using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling.Builders;

namespace Valour.Net.CommandHandling
{
    public class CommandRegistrar
    {
        public CommandRegistrar()
        {
            
        }


        /// <summary>
        /// Finds all classes that inherit from CommandModuleBase and then finds all methods which have a Command attribute
        /// </summary>
        /// MOST OF THIS METHOD IS DEBUG
        public void RegisterAllCommands(CommandService commandService)
        {
            CommandBuilder aa = new CommandBuilder();
            Assembly assembly = Assembly.GetEntryAssembly();

            IEnumerable<Type> exporters = assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(CommandModuleBase)) && !t.IsAbstract);
            //.Select(t => (CommandModuleBase)Activator.CreateInstance(t));

            //DEBUG
            foreach (Type item in exporters)
            {
                ConstructorInfo a = item.GetConstructor(Type.EmptyTypes);
                CommandModuleBase classs = (CommandModuleBase)a.Invoke(Array.Empty<object>());
                //Console.WriteLine(b);
                foreach (var methoddd in classs.GetType().GetMethods())
                {
                    Console.WriteLine(methoddd);

                }

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
