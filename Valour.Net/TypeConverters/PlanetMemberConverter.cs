using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling;

namespace Valour.Net.TypeConverters
{
    public static class PlanetMemberConverter
    {
        public static bool CanConvertFrom(object _object)
        {
            Type sourceType = _object.GetType();
            if (sourceType == typeof(string)) {
                if (_object.ToString().Contains("«@m-")) {
                    return true;
                }
            }
            return false;
        }

        public static object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            CommandContext ctx = (context as CommandArgConverterContext).ctx;
            object result = null;
            string stringValue = (string)value;
            if (!string.IsNullOrEmpty(stringValue))
            {
                if (long.TryParse(stringValue, out long MemberID)) // Input as id
                {
                    result = PlanetMember.FindAsync(MemberID, ctx.Planet.Id).Result;
                }
                else if (stringValue.Substring(0,4) == "«@m-") {
                    string v = stringValue.Replace("«@m-", "");
                    v = v.Replace("»","");
                    if (long.TryParse(v, out MemberID)) {
                        result = PlanetMember.FindAsync(MemberID, ctx.Planet.Id).Result;
                    }
                    else
                    {
                        result = null;
                    }
                }
                }
            else //Input as name
            {
                // need to fix this
                    //Member member = Member.FindAsync(x => x.Planet_Id == ctx.Planet.Id && x.Nickname.ToLower() == stringValue.ToLower());
                    //if (member != null)
                    //{
                    //    result = member;
                    //}
            }
            return result;
        }
    }
}
