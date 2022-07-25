using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
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
        public PlanetChatChannel Channel { get; set;}
        public PlanetMember Member { get; set;}
        public EmbedInteractionEvent Event { get; set;}

        public InteractionContext() { }

        public async Task SetFromImteractionEvent(EmbedInteractionEvent IEvent)
        {
            Planet = await Planet.FindAsync(IEvent.PlanetId);
            Channel = await PlanetChatChannel.FindAsync(IEvent.ChannelId, IEvent.PlanetId);
            Member = await PlanetMember.FindAsync(IEvent.MemberId, IEvent.PlanetId);
            Event = IEvent;
        }

        public async Task ReplyAsync(string content)
        {
            await ValourNetClient.PostMessage(Channel.Id, Planet.Id, content, null);
        }

        public async Task ReplyAsync(EmbedBuilder embedbuilder)
        {
            await ValourNetClient.PostMessage(Channel.Id, Planet.Id, "", embedbuilder.embed);
        }

        public async Task ReplyWithMessagesAsync(int delay, List<string> data) {
            foreach (string content in data) {
                await ValourNetClient.PostMessage(Channel.Id, Planet.Id, content, null);
                await Task.Delay(delay);
            }
        }

    }
}