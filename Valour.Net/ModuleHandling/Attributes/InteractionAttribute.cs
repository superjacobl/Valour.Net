using System;

namespace Valour.Net.CommandHandling.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class InteractionAttribute : Attribute
    {
        public EmbedIteractionEventType EventType { get; }
        public string InteractionFormId { get; }
        public string InteractionElementId { get; }

        public InteractionAttribute(EmbedIteractionEventType eventType, string interactionFormId = null, string interactionElementId = null)
        {
            InteractionFormId = interactionFormId;
            InteractionElementId = interactionElementId;
            EventType = eventType;
        }
    }
}