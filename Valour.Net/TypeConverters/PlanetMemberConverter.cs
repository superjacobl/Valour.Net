using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling;
using Valour.Net.Models;

namespace Valour.Net.TypeConverters
{
    class PlanetMemberConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || sourceType == typeof(ulong)|| base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            CommandContext ctx = (context as CommandArgConverterContext).ctx;
            object result = null;
            string stringValue = (string)value;
            if (!string.IsNullOrEmpty(stringValue))
            {
                if (ulong.TryParse(stringValue, out ulong MemberID)) // Input as id
                {
                    result = Cache.GetPlanetMember(MemberID, ctx.Planet.Id).Result;
                }
                else //Input as name
                {
                    PlanetMember member = Cache.PlanetMemberCache.Values.FirstOrDefault(x => x.Planet_Id == ctx.Planet.Id && x.Nickname.ToLower() == stringValue.ToLower());
                    if (member != null)
                    {
                        result = member;
                    }
                }
            }
           

            return result;
        }
    }

    
}
