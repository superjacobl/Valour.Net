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

        public async Task Set(PlanetMessage msg)
        {
            Planet = await Cache.GetPlanet(msg.Planet_Id);
            Channel = await Cache.GetPlanetChannel(msg.Channel_Id, msg.Planet_Id);
            Member = await msg.GetAuthorAsync();
            Message = msg;
        }

        public async Task ReplyAsync(string content)
        {
            await ValourClient.PostMessage(Channel.Id, Planet.Id, content);
        }

        public async Task ReplyWithMessagesAsync(int delay, List<string> data) {
            foreach (string content in data) {
                await ValourClient.PostMessage(Channel.Id, Planet.Id, content);
                await Task.Delay(delay);
            }
        }

    }
}