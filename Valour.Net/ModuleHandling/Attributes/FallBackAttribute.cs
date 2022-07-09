using System;

namespace Valour.Net.CommandHandling.Attributes
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class FallBackAttribute : Attribute
    {
        /// <summary>
        ///     Tells the command registrar to use this command if no other options have matching arguments
        /// </summary>
        public FallBackAttribute() { }
    }

}
