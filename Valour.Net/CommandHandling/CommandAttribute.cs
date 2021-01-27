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
        /// <summary>
        ///     Gets the text that has been set to be recognized as a command.
        /// </summary>
        public string Text { get; }

        public CommandAttribute(string text)
        {
            Text = text;
        }
    }
}
