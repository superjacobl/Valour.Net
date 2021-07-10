using System;

namespace Valour.Net.CommandHandling.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GroupAttribute : Attribute
    {
        public string Prefix { get; }

        public GroupAttribute()
        {
            Prefix = null;
        }
        public GroupAttribute(string prefix)
        {
            Prefix = prefix;
        }
    }
}