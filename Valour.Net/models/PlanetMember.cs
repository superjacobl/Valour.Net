using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Valour.Net.TypeConverters;
using Valour.Api.Items.Planets;
using Valour.Api.Client;

namespace Valour.Net.Models
{
    [TypeConverter(typeof(PlanetMemberConverter))]
    public class _NetMember : PlanetMember
    {
        public List<ulong> RoleIds
        {
            get
            {
                return (List<ulong>)GetRolesAsync().Result.Select(x => x.Id);
            }
            set { }
        }

        public List<PlanetRole> Roles
        {
            get
            {
                return (GetRolesAsync()).Result;
            }
            set { }
        }

        public Planet Planet
        {
            get
            {
                return (Planet.FindAsync(this.Planet_Id)).Result;
            }
        }

        public async Task UpdateRoles() {
            await this.GetRolesAsync(true);
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
        /// <summary>
        /// If all is true, then the member must have all roles
        /// </summary>
        /// <returns></returns>
        public bool HasRoles(IEnumerable<PlanetRole> TestRoles, bool All = false) {
            if (All) {
                foreach (PlanetRole role in TestRoles) {
                    if (!Roles.Any(x => x == role)) {
                        return false;
                    }
                }
                return true;
            }
            else {
                foreach (PlanetRole role in TestRoles) {
                    if (Roles.Any(x => x == role)) {
                        return true;
                    }
                }
                return false;
            }
        }

        public static async Task<_NetMember> FindAsync(ulong id)
        {
            PlanetMember member = await PlanetMember.FindAsync(id);
            return (_NetMember)member;
        }

        public async Task<bool> IsOwner()
        {
            // add role-based authority later
            if (Id == (await Planet.FindAsync(this.Planet_Id)).Owner_Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SetRoleMembership(PlanetRole role, bool value) {
            if (value)
            {
          
            }
            else
            {

            }
            //ValourResponse<string> response = await ValourClient.GetResponse<string>($"https://valour.gg/Planet/SetMemberRoleMembership?role_id={role.Id}&member_id={Id}&value={value}&token={ValourClient.Token}");
            return true;//response.Success;
        }
        public async Task<bool> AddRoleAsync(string name) {
            PlanetRole role = (await this.Planet.GetRolesAsync()).FirstOrDefault(x => x.Name == name);
            if (role == null) {
                return false;
            }
            return await SetRoleMembership(role, true);
        }
        public async Task<bool> AddRoleAsync(ulong roleid) {
            PlanetRole role = (await this.Planet.GetRolesAsync()).FirstOrDefault(x => x.Id == roleid);
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
        public async Task AddRolesAsync(IEnumerable<PlanetRole> roles) {
            foreach (PlanetRole role in roles) {
                await SetRoleMembership(role, true);
            }
        }
        public async Task<bool> RemoveRoleAsync(string name) {
            PlanetRole role = (await this.Planet.GetRolesAsync()).FirstOrDefault(x => x.Name == name);
            if (role == null) {
                return false;
            }
            return await SetRoleMembership(role, false);
        }
        public async Task<bool> RemoveRoleAsync(ulong roleid) {
            PlanetRole role = (await this.Planet.GetRolesAsync()).FirstOrDefault(x => x.Id == roleid);
            if (role == null) {
                return false;
            }
            return await SetRoleMembership(role, false);
        }
        public async Task<bool> RemoveRoleAsync(PlanetRole role) {
            return await SetRoleMembership(role, false);
        }

        public async Task RemoveRolesAsync(IEnumerable<PlanetRole> roles) {
            foreach (PlanetRole role in roles) {
                await SetRoleMembership(role, false);
            }
        }
        // add ban
        public async Task BanAsync()
        {

        }

    }
}
