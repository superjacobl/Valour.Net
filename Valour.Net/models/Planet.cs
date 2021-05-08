using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace Valour.Net.Models
{
    public class Planet
    {
        /// <summary>
        /// The ID of the planet
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// The Id of the owner of this planet
        /// </summary>
        public ulong Owner_Id { get; set; }

        /// <summary>
        /// The name of the planet
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The image url for the planet 
        /// </summary>
        public string Image_Url { get; set; }

        /// <summary>
        /// The description of the planet
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// If the server requires express allowal to join a planet
        /// </summary>
        public bool Public { get; set; }

        /// <summary>
        /// The amount of members on the planet
        /// </summary>
        public uint Member_Count { get; set; }

        /// <summary>
        /// The default role for the planet
        /// </summary>
        public ulong Default_Role_Id { get; set; }

        /// <summary>
        /// The id of the main channel of the planet
        /// </summary>
        public ulong Main_Channel_Id { get; set; }

        public List<PlanetRole> Roles { get; set; }

        public async Task GetRoles() {
            Roles = await ValourClient.GetData<List<PlanetRole>>($"https://valour.gg/Planet/GetPlanetRoles?planet_id={Id}&token={ValourClient.Token}");
        }

        public async Task<List<Channel>> GetChannelsAsync() {
            return Cache.ChannelCache.Values.Where(x => x.Planet_Id == Id).ToList();
        }

        public async Task<List<PlanetMember>> GetMembers() {
            return Cache.PlanetMemberCache.Values.Where(x => x.Planet_Id == Id).ToList();
        }

        public async Task<PlanetMember> GetMember(ulong UserId, ulong PlanetId) {
            return Cache.PlanetMemberCache.Values.First(x => x.Planet_Id == Id && x.User_Id == UserId);
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

    }
}