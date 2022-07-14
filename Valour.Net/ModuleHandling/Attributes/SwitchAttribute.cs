using System;

namespace Valour.Net.CommandHandling.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class SwitchInputAttribute : Attribute
    {
        public string SwitchName { get; }

        public SwitchInputAttribute(string switchName)
        {
            SwitchName = switchName;
        }
    }
}