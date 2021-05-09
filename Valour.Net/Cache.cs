using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Valour.Net.Models;
using System.Collections.Concurrent;

namespace Valour.Net
{
    public class Cache
    {
        
        public static ConcurrentDictionary<ulong, Channel> ChannelCache = new ConcurrentDictionary<ulong, Channel>();
        public static ConcurrentDictionary<ulong, Planet> PlanetCache = new ConcurrentDictionary<ulong, Planet>();
        public static ConcurrentDictionary<ulong, PlanetMember> PlanetMemberCache = new ConcurrentDictionary<ulong, PlanetMember>();

        public static async Task<PlanetMember> GetPlanetMember(ulong UserId, ulong PlanetId) {
            PlanetMember member = PlanetMemberCache.Values.FirstOrDefault(x => x.Planet_Id == PlanetId && x.User_Id == UserId);
            if (member == null) {
                member = await ValourClient.GetData<PlanetMember>($"https://valour.gg/Planet/GetPlanetMember?user_id={UserId}&planet_id={PlanetId}&auth={ValourClient.Token}");
                PlanetMemberCache.TryAdd(member.Id, member);
            }
            return member;
        }

        public static async Task<Channel> GetPlanetChannel(ulong ChannelId, ulong PlanetId)
        {
            Channel channel = ChannelCache.Values.FirstOrDefault(x => x.Id == ChannelId);
            if (channel == null)
            {
                // this will be later, when Valour api has /GetChannel
                // channel = await ValourClient.GetData<Channel>($"https://valour.gg/Channel/GetChannel?channel_id={ChannelId}&auth={ValourClient.Token}");
                // ChannelCache.Add(channel.Id, channel);
                await UpdateChannelsFromPlanetAsync(PlanetId);
                return ChannelCache.Values.FirstOrDefault(x => x.Id == ChannelId);
            }
            return channel;
        }

        public static async Task<Planet> GetPlanet(ulong PlanetId)
        {
            Planet planet = PlanetCache.Values.FirstOrDefault(x => x.Id == PlanetId);
            if (planet == null)
            {
                planet = await ValourClient.GetData<Planet>($"https://valour.gg/Planet/GetPlanet?planet_id={PlanetId}&auth={ValourClient.Token}");
                PlanetCache.TryAdd(planet.Id, planet);
                return PlanetCache.Values.FirstOrDefault(x => x.Id == PlanetId);
            }
            return planet;
        }

        public static async Task UpdatePlanetRoles(ulong PlanetId)
        {
            await PlanetCache.Values.FirstOrDefault(x => x.Id == PlanetId).GetRoles();
        }

        public static async Task UpdateChannelsFromPlanetAsync(ulong PlanetId)
        {
            foreach (Channel channel in await ValourClient.GetData<List<Channel>>($"https://valour.gg/Channel/GetPlanetChannels?planet_id={PlanetId}&token={ValourClient.Token}")) {
                if (ChannelCache.ContainsKey(channel.Id) == false) {
                    ChannelCache.TryAdd(channel.Id, channel);
                }
            }
        }

        public static async Task UpdateMembersFromPlanetAsync(ulong PlanetId)
        {
            foreach (PlanetMemberInfo memberinfo in await ValourClient.GetData<List<PlanetMemberInfo>>($"https://valour.gg/Planet/GetPlanetMemberInfo?planet_id={PlanetId}&token={ValourClient.Token}")) {
                if (PlanetMemberCache.ContainsKey(memberinfo.Member.Id) == false) {
                    PlanetMember member = memberinfo.Member;
                    member.RoleIds = new List<ulong>();
                    member.RoleIds.AddRange(memberinfo.RoleIds);
                    PlanetMemberCache.TryAdd(memberinfo.Member.Id, member);
                }
            }
        }

        public static async Task UpdatePlanetAsync() 
        {
            foreach (Planet planet in await ValourClient.GetData<List<Planet>>($"https://valour.gg/Planet/GetPlanetMembership?user_id={ValourClient.BotId}&token={ValourClient.Token}")) {
                if (PlanetCache.ContainsKey(planet.Id) == false) {
                    PlanetCache.TryAdd(planet.Id, planet);
                }
            }
        }

    }
}