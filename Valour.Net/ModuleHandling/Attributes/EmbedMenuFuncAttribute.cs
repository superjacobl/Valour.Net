using System;

namespace Valour.Net.CommandHandling.Attributes
{

    /// <summary>
    /// Tells Valour.Net that the function can be targeted using .OnClick
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class EmbedMenuFuncAttribute : Attribute
    {
        public EmbedMenuFuncAttribute()
        {
        }
    }
}