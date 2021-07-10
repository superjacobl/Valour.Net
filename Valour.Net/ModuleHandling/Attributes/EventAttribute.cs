using System;

namespace Valour.Net.CommandHandling
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class EventAttribute : Attribute
    {
        public string Name { get; }
        public EventAttribute(string name)
        {
            Name = name;
        }
    }
}
