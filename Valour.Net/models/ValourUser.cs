using System;
using System.ComponentModel;
using Valour.Net.TypeConverters;
using Newtonsoft.Json;

namespace Valour.Net.Models
{
    [TypeConverter(typeof(ValourUserConverter))]
    [JsonObject]
    public class ValourUser
    {
        public ulong Id { get; set; }
        public string Username { get; set; }

        public string Pfp_Url { get; set; }

        public DateTime Join_DateTime { get; set; }
        public bool Bot { get; set; }
        public bool Disabled { get; set; }
        public bool Valour_Staff { get; set; }

        public int UserState_Value { get; set; }
        public DateTime Last_Active { get; set; }
    }
}
