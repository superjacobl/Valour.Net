using System;

namespace Valour.Net.CommandHandling
{
    /// <summary> Theses roles will not be able to run a command. </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExpectRoleAttribute : Attribute
    {
        /// <summary> The roles which will not be able to run this command </summary>
        public string[] Roles { get; }

        /// <summary> Creates a new <see cref="ExpectRoleAttribute"/> with the given roles. </summary>
        public ExpectRoleAttribute(params string[] roles)
        {
            Roles = roles;
        }
    }
}
