using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        public void RegisterAllCommands()
        {
            Assembly assembly = Assembly.GetEntryAssembly();


            MethodInfo[] methods = assembly.GetTypes()
                    .SelectMany(t => t.GetMethods())
                    .Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0)
                    .ToArray();

                    //.Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0)
            foreach (MethodInfo method in methods)
            {
                CommandAttribute myAttributes = (CommandAttribute)Attribute.GetCustomAttribute(method,typeof(CommandAttribute));
                Console.WriteLine($"Method {method.Name} has a command attribute with the text {myAttributes.Name}"); //DEBUG
                foreach (var param in method.GetParameters())
                {
                    Console.WriteLine($"Method {method.Name} has a parameter {param.Name} with type {param.ParameterType}");
                }
                
            }
        }
    }
}
