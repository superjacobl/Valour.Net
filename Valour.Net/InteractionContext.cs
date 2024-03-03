using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Collections.Concurrent;
using Valour.Net;
using Valour.Sdk.Client;
using System.Text.Json;
using Valour.Sdk.Models.Messages.Embeds.Items;

namespace Valour.Net.CommandHandling
{
    public class InteractionContext : IContext
    {
        public EmbedInteractionEvent Event { get; set;}

        public InteractionContext() { }

        public async Task SetFromImteractionEvent(EmbedInteractionEvent IEvent)
        {
            Planet = await Planet.FindAsync(IEvent.PlanetId);
            Channel = await Channel.FindAsync(IEvent.ChannelId, IEvent.PlanetId);
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
                NewEmbedContent = JsonSerializer.Serialize(embed.embed, options),
                ChangedEmbedItemsContent = null
            };
            if (targetuserid != 0)
                peu.TargetUserId = targetuserid;
            var result = await ValourClient.PostAsyncWithResponse<string>($"api/embed/planetpersonalupdate", JsonSerializer.Serialize(peu, options), Channel.Node.HttpClient);
            //Console.WriteLine($"took: {DateTime.UtcNow.Subtract(Event.TimeInteracted).TotalMilliseconds}ms");
            //Console.WriteLine(peu.NewEmbedContent);
            if (false)
            {
            }
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
            await ValourClient.PostAsyncWithResponse<string>($"api/embed/planetchannelupdate", JsonSerializer.Serialize(ceu, options), Channel.Node.HttpClient);
        }

        /// <summary>
        /// Sends a targeted embed update to the user who sent this interaction, or if targetuserid is set, then to the targetuserid.
        /// This will only send changes made to the embed, not the entire embed!
        /// </summary>
        public async Task SendTargetedEmbedUpdateForUser(EmbedBuilder embed, long targetuserid = 0)
        {
            if (embed.Data.ContainsKey("sendfullupdate") && (bool)embed.Data["sendfullupdate"])
            {
                await UpdateEmbedForUser(embed, targetuserid);
                return;
            }
            JsonSerializerOptions options = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };
            //var user = await Member.GetUserAsync();
            List<EmbedItem> items = new();
            foreach (var page in embed.embed.Pages)
            {
                foreach (var item in page.GetAllItems())
                {
                    if (item.ExtraData != null && item.ExtraData.ContainsKey("HasChanged") && (bool)item.ExtraData["HasChanged"])
                        items.Add(item);
                    else
                    {

                    }    
                }
            }

            var _items = items.ToList();
            foreach (var item in _items)
            {
                //Console.WriteLine($"Update for {item.ItemType} with id {item.Id}");
                if (item.ExtraData is not null && item.ExtraData.ContainsKey("IsMostLikeyNew") && (bool)item.ExtraData["IsMostLikeyNew"])
                    continue;
                if (false && _items.Any(x => x.GetAllItems().Contains(item)))
                    items.Remove(item);
            }

            PersonalEmbedUpdate peu = new()
            {
                TargetUserId = Member.UserId,
                TargetMessageId = Event.MessageId,
                NewEmbedContent = null,
                ChangedEmbedItemsContent = JsonSerializer.Serialize(items, options)
            };
            //Console.WriteLine(peu.ChangedEmbedItemsContent);
            if (targetuserid != 0)
                peu.TargetUserId = targetuserid;
            var data = JsonSerializer.Serialize(peu, options);
            await ValourClient.PostAsyncWithResponse<string>($"api/embed/planetpersonalupdate", data, Channel.Node.HttpClient);
            //var result = await ValourClient.PostAsyncWithResponse<string>($"api/embed/planetpersonalupdate", data, user.Node.HttpClient);
            //Console.WriteLine($"took: {DateTime.UtcNow.Subtract(Event.TimeInteracted).TotalMilliseconds}ms");
            if (false)
            {
            }
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