using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Collections.Concurrent;
using Valour.Net;
using Valour.Api.Items.Messages.Embeds;
using Valour.Api.Items.Users;
using Valour.Api.Client;
using System.Text.Json;

namespace Valour.Net.CommandHandling
{
    public class InteractionContext : IContext
    {
        public EmbedInteractionEvent Event { get; set;}

        public InteractionContext() { }

        public async Task SetFromImteractionEvent(EmbedInteractionEvent IEvent)
        {
            Planet = await Planet.FindAsync(IEvent.PlanetId);
            Channel = await PlanetChatChannel.FindAsync(IEvent.ChannelId, IEvent.PlanetId);
            Member = await PlanetMember.FindAsync(IEvent.MemberId, IEvent.PlanetId);
            Event = IEvent;
        }

        /// <summary>
        /// Sends a embed update to the user who sent this interaction, or if targetuserid is set, then to the targetuserid
        /// </summary>
        public async Task UpdateEmbedForUser(EmbedBuilder embed, long targetuserid = 0)
        {
            JsonSerializerOptions options = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };
            PersonalEmbedUpdate peu = new()
            {
                TargetUserId = Member.UserId,
                TargetMessageId = Event.MessageId,
                NewEmbedContent = JsonSerializer.Serialize(embed.embed, options)
            };
            if (targetuserid != 0)
                peu.TargetUserId = targetuserid;
            await ValourClient.PostAsyncWithResponse<string>($"api/embed/planetpersonalupdate", JsonSerializer.Serialize(peu, options));
        }

        /// <summary>
        /// Sends a embed update to the channel who sent this interaction, or if targetchannelid is set, then to the targetchannelid
        /// </summary>
        public async Task UpdateEmbedForChannel(EmbedBuilder embedBuilder, long targetchannelid = 0) 
        {
            JsonSerializerOptions options = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };
            ChannelEmbedUpdate ceu = new() {
                TargetChannelId = Channel.Id,
                TargetMessageId = Event.MessageId,
                NewEmbedContent = JsonSerializer.Serialize(embedBuilder.embed, options)
            };
            if (targetchannelid != 0)
                ceu.TargetChannelId = targetchannelid;
            await ValourClient.PostAsyncWithResponse<string>($"api/embed/planetchannelupdate", JsonSerializer.Serialize(ceu, options));
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