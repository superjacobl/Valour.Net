﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling;

namespace Valour.Net.TypeConverters
{
    class ValourUserConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || sourceType == typeof(long)|| base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            CommandContext ctx = (context as CommandArgConverterContext).ctx;

            object result = null;
            string stringValue = (string)value;
            if (!string.IsNullOrEmpty(stringValue))
            {
                
                if (long.TryParse(stringValue, out long UserID)) // Input as id
                {
                    //result = Cache.GetValourUser(UserID).Result;
                }
                else if (stringValue.Substring(0, 4) == "«@m-") //input as ping 
                {
                    if (long.TryParse(stringValue.Substring(4, 15), out long MemberID))
                    {
                        
                        //result = Cache.GetValourUser(Cache.PlanetMemberCache.Values.FirstOrDefault(x => x.Planet_Id == ctx.Planet.Id && x.Id == MemberID).User_Id).Result;
                    }
                    else
                    {
                        result = null;
                    }

                }
                else //Input as name
                {
                    //PlanetMember member = Cache.PlanetMemberCache.Values.FirstOrDefault(x => x.Nickname == stringValue);
                    //if (member != null)
                    //{
                        //result = Cache.GetValourUser(member.User_Id).Result;
                    //}
                }
            }
           

            return result;
        }
    }

    
}
