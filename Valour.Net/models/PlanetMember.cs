using System.Collections.Generic;
using System.Threading.Tasks;

namespace Valour.Net.Models
{
    public class PlanetMember
    {
        public ulong Planet_Id { get; set;}
        public ulong Id { get; set; }
        public ulong User_Id { get; set; }
        public string Nickname { get; set; }
        public string Member_Pfp { get; set; }
        public List<ulong> RoleIds { get; set; }
        public List<string> RolesNames = new List<string>();

        public async Task<bool> IsOwner() {
            // add role-based authority later
            if (Nickname == "superjacobl") {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
