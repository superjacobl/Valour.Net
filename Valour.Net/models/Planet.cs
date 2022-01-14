using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Valour.Api.Items.Planets;
using Valour.Api.Client;

namespace Valour.Net.Models.Planets
{
    public class NetPlanet : Planet
    {
        public PlanetRole GetRole(string RoleName) {
            return this.GetRolesAsync().Result.FirstOrDefault(x => x.Name == RoleName);
        }
        public PlanetRole GetRole(ulong RoleId) {
            return this.GetRolesAsync().Result.FirstOrDefault(x => x.Id == RoleId);
        }
        public PlanetRole GetRole(PlanetRole Role) {
            return this.GetRolesAsync().Result.FirstOrDefault(x => x == Role);
        }
    }
}