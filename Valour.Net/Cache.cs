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
        public static ConcurrentDictionary<ulong, PlanetRole> PlanetRoleCache = new ConcurrentDictionary<ulong, PlanetRole>();
        public static ConcurrentDictionary<ulong, PlanetMember> PlanetMemberCache = new ConcurrentDictionary<ulong, PlanetMember>();
        public static ConcurrentDictionary<ulong, ValourUser> ValourUserCache = new ConcurrentDictionary<ulong, ValourUser>();


        public static async Task<PlanetMember> GetPlanetMember(ulong UserId, ulong PlanetId) {
            PlanetMember member = PlanetMemberCache.Values.FirstOrDefault(x => x.Planet_Id == PlanetId && x.User_Id == UserId);
            if (member == null) {
                member = await ValourClient.GetData<PlanetMember>($"https://valour.gg/Planet/GetPlanetMember?user_id={UserId}&planet_id={PlanetId}&auth={ValourClient.Token}");
                if (member == null) {
                    return null;
                }
                await member.UpdateRoles();
                PlanetMemberCache.TryAdd(member.Id, member);
            }
            return member;
        }

        public static async Task<ValourUser> GetValourUser(ulong UserId)
        {
            ValourUser User = ValourUserCache.Values.FirstOrDefault(x => x.Id == UserId);
            if (User == null)
            {
                User = await ValourClient.GetData<ValourUser>($"https://valour.gg/User/GetUser?id={UserId}");
                if (User == null)
                {
                    return null;
                }
                ValourUserCache.TryAdd(User.Id, User);
            }
            return User;
        }

        public static Channel GetPlanetChannel(ulong ChannelId, ulong PlanetId)
        {
            Channel channel = ChannelCache.Values.FirstOrDefault(x => x.Id == ChannelId);
            return channel;
        }

        public static async Task<Channel> GetPlanetChannelAsync(ulong ChannelId, ulong PlanetId)
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
            Planet planet = await GetPlanet(PlanetId);
            await planet.GetRoles();
            planet.Roles.ForEach(x => PlanetRoleCache.AddOrUpdate(x.Id, x, (key, oldValue) => oldValue));
        }

        public static async Task UpdateChannelsFromPlanetAsync(ulong PlanetId)
        {
            Parallel.ForEach(await ValourClient.GetData<List<Channel>>($"https://valour.gg/Channel/GetPlanetChannels?planet_id={PlanetId}&token={ValourClient.Token}"), channel =>
            {
                ChannelCache.TryAdd(channel.Id, channel);
            }); 
        }

        public static async Task UpdateMembersFromPlanetAsync(ulong PlanetId)
        {
            Planet planet = await GetPlanet(PlanetId);
            Parallel.ForEach(await ValourClient.GetData<List<PlanetMemberInfo>>($"https://valour.gg/Planet/GetPlanetMemberInfo?planet_id={PlanetId}&token={ValourClient.Token}"), memberinfo => 
            {
                if (PlanetMemberCache.ContainsKey(memberinfo.Member.Id) == false)
                {
                    PlanetMember member = memberinfo.Member;
                    member.RoleIds = new List<ulong>();
                    member.RoleIds.AddRange(memberinfo.RoleIds);
                    PlanetMemberCache.TryAdd(memberinfo.Member.Id, member);
                    ValourUserCache.TryAdd(memberinfo.Member.User_Id, memberinfo.User);
                }
            });
        }


        public static async Task UpdatePlanetAsync() 
        {
            Parallel.ForEach(await ValourClient.GetData<List<Planet>>($"https://valour.gg/Planet/GetPlanetMembership?user_id={ValourClient.BotId}&token={ValourClient.Token}"), planet =>
            {
                PlanetCache.TryAdd(planet.Id, planet);
            });

        }

    }
}