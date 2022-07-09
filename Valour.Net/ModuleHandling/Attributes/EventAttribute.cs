using System;

namespace Valour.Net.CommandHandling.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class EventAttribute : Attribute
{
    public EventType eventType { get; }
    public EventAttribute(EventType eventtype)
    {
        eventType = eventtype;
    }
}