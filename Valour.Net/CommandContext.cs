using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Valour.Net.Models;
using System.Collections.Concurrent;
using Valour.Net;

namespace Valour.Net.CommandHandling
{
    public class CommandContext
    {
        public Planet Planet { get; set;}
        public Channel Channel { get; set;}
        public PlanetMember Member { get; set;}
        public PlanetMessage Message { get; set;}

        public CommandContext() { }

        async Task Set(PlanetMessage msg)
        {
            Planet = await Cache.GetPlanet(msg.Planet_Id);
            Channel = await Cache.GetPlanetChannelAsync(msg.Channel_Id, msg.Planet_Id);
            Member = await msg.GetAuthorAsync();
            Message = msg;
        }

        public async Task SetFromImteractionEvent(InteractionEvent IEvent)
        {
            Planet = await Cache.GetPlanet(IEvent.Planet_Id);
            Channel = await Cache.GetPlanetChannelAsync(IEvent.Channel_Id, IEvent.Planet_Id);
            Member = Cache.PlanetMemberCache.First(x => x.Key == IEvent.Author_Member_Id).Value;
            Message = null;
        }

        public async Task ReplyAsync(string content)
        {
            await ValourClient.PostMessage(Channel.Id, Planet.Id, content, null);
        }

        public async Task ReplyAsync(ClientEmbed embed)
        {
            await ValourClient.PostMessage(Channel.Id, Planet.Id, "", embed);
        }
        public async Task ReplyWithMessagesAsync(int delay, List<string> data) {
            foreach (string content in data) {
                await ValourClient.PostMessage(Channel.Id, Planet.Id, content, null);
                await Task.Delay(delay);
            }
        }

    }
}