using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valour.Net.CommandHandling
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CommandAttribute : Attribute
    {
        
        public string Name { get; }
        public string[] Aliases { get; }
        /// <summary>
        ///     Gets the text that has been set to be recognized as a command.
        /// </summary>
        /// <param name="name">The main name for the command</param>
        public CommandAttribute(string name)
        {
            Name = name;
        }
        /// <summary>
        ///     Gets the text that has been set to be recognized as a command.
        /// </summary>
        /// <param name="name">The main name for the command</param>
        /// <param name="Aliases">Aliases for this command</param>
        public CommandAttribute(string name, params string[] Aliases)
        {
            Name = name;
            this.Aliases = Aliases;
        }
    }
}
