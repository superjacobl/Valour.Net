using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Valour.Net.Models.Planets;
using Valour.Net.Models.Messages;
using System.Collections.Concurrent;
using Valour.Net;
using Valour.Api.Items.Messages;
using Valour.Api.Items.Planets.Channels;
using Valour.Api.Items.Planets.Members;
using Valour.Api.Items.Users;
using Valour.Api.Items.Planets;
using Valour.Api.Client;
using Valour.Net.Models;
using Valour.Shared.Items.Messages;

namespace Valour.Net.CommandHandling
{
    public class CommandContext
    {
        public Planet Planet { get; set;}
        public PlanetChatChannel Channel { get; set;}
        public PlanetMember Member { get; set;}
        public PlanetMessage Message { get; set;}
        public TimeSpan MessageTimeTook { get; set; }

        public CommandContext() { }

        public async Task Set(PlanetMessage msg)
        {
            Channel = await PlanetChatChannel.FindAsync(msg.Channel_Id);
            Member = await PlanetMember.FindAsync(msg.Member_Id);
            Message = msg;
            Planet = await Planet.FindAsync(Channel.Planet_Id);
        }

        public async Task ReplyAsync(string content)
        {
            await ValourNetClient.PostMessage(Channel.Id, Planet.Id, content, null);
        }

        public async Task ReplyAsync(EmbedBuilder embed)
        {
            await ValourNetClient.PostMessage(Channel.Id, Planet.Id, "", embed.Generate());
        }
        public async Task ReplyWithMessagesAsync(int delay, List<string> data) {
            foreach (string content in data) {
                await ValourNetClient.PostMessage(Channel.Id, Planet.Id, content, null);
                await Task.Delay(delay);
            }
        }

    }
}