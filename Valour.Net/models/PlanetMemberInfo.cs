using System.Collections.Generic;

namespace Valour.Net.Models
{
    public class PlanetMemberInfo
    {
        public PlanetMember Member { get; set; }
        public string State { get; set; }
        public List<ulong> RoleIds { get; set; }
    }
}
