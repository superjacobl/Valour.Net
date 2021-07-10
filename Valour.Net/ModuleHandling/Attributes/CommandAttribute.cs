using System;

namespace Valour.Net.CommandHandling
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CommandAttribute : Attribute
    {
        public string Name { get; }

        /// <summary>
        ///     Gets the text that has been set to be recognized as a command.
        /// </summary>
        /// <param name="name">The main name for the command</param>
        /// 
        public CommandAttribute(string name)
        {
            Name = name;
        }
    }
}
