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
        public void GetAllCommands()
        {
            Assembly assembly = Assembly.GetEntryAssembly();


            MemberInfo[] methods = assembly.GetTypes()
                    .SelectMany(t => t.GetMembers())
                    .Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0)
                    .ToArray();

                    //.Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0)
            foreach (var method in methods)
            {
                CommandAttribute myAttributes = (CommandAttribute)Attribute.GetCustomAttribute(method,typeof(CommandAttribute));
                Console.WriteLine($"Method {method.Name} has a command attribute with the text {myAttributes.Text}"); //DEBUG
                
            }
        }
    }
}
