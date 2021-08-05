using System;

namespace Valour.Net.CommandHandling
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class InteractionAttribute : Attribute
    {
        public string InteractionName { get; }

        public string InteractionID { get; }
        public InteractionAttribute(string name)
        {
            InteractionName = name;
            InteractionID = null;
        }

        public InteractionAttribute(string InteractionName, string ElementID)
        {
            this.InteractionName = InteractionName;
            InteractionID = ElementID;
        }
    }
}
