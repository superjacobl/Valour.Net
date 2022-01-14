using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valour.Api.Items.Planets;
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
                    result = PlanetMember.FindAsync(ctx.Planet.Id, MemberID).Result;
                }
                else if (stringValue.Substring(0,4) == "«@m-") //input as ping 
                {
                    if (ulong.TryParse(stringValue.Substring(4, 15), out MemberID))
                    {
                        result = PlanetMember.FindAsync(MemberID, ctx.Planet.Id).Result;
                    }
                    else
                    {
                        result = null;
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
            }
           

            return result;
        }
    }

    
}
