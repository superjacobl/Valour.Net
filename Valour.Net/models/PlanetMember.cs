using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Valour.Net.Models
{
    public class PlanetMember
    {
        public ulong Planet_Id { get; set;}
        public ulong Id { get; set; }
        public ulong User_Id { get; set; }
        public string Nickname { get; set; }
        public string Member_Pfp { get; set; }
        public List<ulong> RoleIds {get; set;}
        public List<PlanetRole> Roles = new List<PlanetRole>();

        public async Task<bool> IsOwner() {
            // add role-based authority later
            if (Id == (await Cache.GetPlanet(Planet_Id)).Owner_Id) {
                return true;
            }
            else {
                return false;
            }
        }

        public async Task<PlanetRole> GetRole(string RoleName) {
            return Roles.FirstOrDefault(x => x.Name == RoleName);
        }
        public async Task<PlanetRole> GetRole(ulong RoleId) {
            return Roles.FirstOrDefault(x => x.Id == RoleId);
        }
        public async Task<PlanetRole> GetRole(PlanetRole Role) {
            return Roles.FirstOrDefault(x => x == Role);
        }
        public bool HasRole(string RoleName) {
            return Roles.Any(x => x.Name == RoleName);
        }
        public bool HasRole(ulong RoleId) {
            return Roles.Any(x => x.Id == RoleId);
        }
        public bool HasRole(PlanetRole Role) {
            return Roles.Any(x => x == Role);
        }

    }
}
