using System;

namespace Valour.Net.CommandHandling.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class InteractionAttribute : Attribute
    {
        public EmbedIteractionEventType EventType { get; }
        public string InteractionName { get; }
        public string InteractionID { get; }

        public InteractionAttribute(EmbedIteractionEventType eventType, string interactionName = null, string ElementID = null)
        {
            InteractionName = InteractionName;
            InteractionID = ElementID;
            EventType = eventType;
        }
    }
}