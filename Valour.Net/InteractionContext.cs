using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Valour.Net.Models.Planets;
using System.Collections.Concurrent;
using Valour.Net;
using Valour.Api.Items.Messages;
using Valour.Api.Items.Users;
using Valour.Api.Client;

namespace Valour.Net.CommandHandling
{
    public class InteractionContext
    {
        public Planet Planet { get; set;}
        public NetChannel Channel { get; set;}
        public PlanetMember Member { get; set;}
        public EmbedInteractionEvent Event { get; set;}

        public InteractionContext() { }

        public async Task SetFromImteractionEvent(EmbedInteractionEvent IEvent)
        {
            Planet = await Planet.FindAsync(IEvent.Planet_Id);
            Channel = (NetChannel)await PlanetChatChannel.FindAsync(IEvent.Channel_Id);
            Member = await PlanetMember.FindAsync(IEvent.Member_Id);
            Event = IEvent;
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