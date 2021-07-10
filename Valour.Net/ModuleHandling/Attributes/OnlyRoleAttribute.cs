using System;

namespace Valour.Net.CommandHandling
{
    /// <summary> Only theses roles will be able to run a command. </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class OnlyRoleAttribute : Attribute
    {
        /// <summary> The roles which will be able to run this command </summary>
        public string[] Roles { get; }

        /// <summary> Creates a new <see cref="OnlyRoleAttribute"/> with the given roles. </summary>
        public OnlyRoleAttribute(params string[] roles)
        {
            Roles = roles;
        }
    }
}
