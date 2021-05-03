using System;

namespace Valour.Net.CommandHandling.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DontAutoLoadAttribute : Attribute
    {
        /// <summary>
        ///     Tells the command registrar to ignore this command when auto-registering commands on startup.
        /// </summary>
        public DontAutoLoadAttribute() { }
    }
}
