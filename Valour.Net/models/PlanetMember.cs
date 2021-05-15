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

        public async Task UpdateRoles() {
            Roles = await ValourClient.GetData<List<PlanetRole>>($"https://valour.gg/Planet/GetMemberRoles?member_id={Id}&token={ValourClient.Token}");
            RoleIds = Roles.Select(x => x.Id).ToList();
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
        
        public async Task<bool> SetRoleMembership(PlanetRole role, bool value) {
            ValourResponse<string> response = await ValourClient.GetResponse<string>($"https://valour.gg/Planet/SetMemberRoleMembership?role_id={role.Id}&member_id={Id}&value={value}&token={ValourClient.Token}");
            return response.Success;
        }
        public async Task<bool> AddRoleAsync(string name) {
            PlanetRole role = Cache.PlanetRoleCache.Values.FirstOrDefault(x => x.Name == name);
            if (role == null) {
                return false;
            }
            return await SetRoleMembership(role, true);
        }
        public async Task<bool> AddRoleAsync(ulong roleid) {
            PlanetRole role = Cache.PlanetRoleCache.Values.FirstOrDefault(x => x.Id == roleid);
            if (role == null) {
                return false;
            }
            return await SetRoleMembership(role, true);
        }
        public async Task<bool> AddRoleAsync(PlanetRole role) {
            if (role == null) {
                return false;
            }
            return await SetRoleMembership(role, true);
        }
        public async Task<bool> RemoveRoleAsync(string name) {
            PlanetRole role = Cache.PlanetRoleCache.Values.FirstOrDefault(x => x.Name == name);
            if (role == null) {
                return false;
            }
            return await SetRoleMembership(role, true);
        }
        public async Task<bool> RemoveRoleAsync(ulong roleid) {
            PlanetRole role = Cache.PlanetRoleCache.Values.FirstOrDefault(x => x.Id == roleid);
            if (role == null) {
                return false;
            }
            return await SetRoleMembership(role, true);
        }
        public async Task<bool> RemoveRoleAsync(PlanetRole role) {
            if (role == null) {
                return false;
            }
            return await SetRoleMembership(role, true);
        }

    }
}
