using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Valour.Sdk.Models.Messages.Embeds.Items;
using Valour.Net.CommandHandling;

namespace Valour.Net.EmbedMenu;

public static class EmbedMenuExtensions
{
    public static JsonSerializerOptions options = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };

    /// <summary>
    /// You must use [EmbedFuncMenu] on the function you are targeting!
    /// </summary>
    /// <param name="embed"></param>
    /// <param name="onClick">The function to be executed upon a user clicking the item</param>
    /// <param name="extraid">Any additional data to be sent; ex: if item is button, if it's for selling or buying</param>
    public static EmbedBuilder OnClick(this EmbedBuilder embed, Func<InteractionContext, ValueTask> onClick, string extraid = " ")
    {
        var item = embed.LastItem;//embed.embed.GetLastItem(true);
        var name = $"MENU$-{onClick.Method.Module.Name}.{onClick.Method.Name}";
        switch (item.ItemType)
        {
            case EmbedItemType.Text:
                embed.OnClickSendInteractionEvent($"{extraid}::{name}");
                break;
            case EmbedItemType.Button:
                embed.OnClickSendInteractionEvent($"{extraid}::{name}");
                break;
        }
        //EmbedMenuManager.ElementIdsToFuncs[$"MENU$-{EmbedMenuManager.ElementIdsToFuncs.Count}"] = onClick;
        //EmbedMenuManager.ElementIdsToFuncs[name] = onClick;
        return embed;
    }

    public static EmbedBuilder HasChanged(this EmbedBuilder embed, bool HasChanged)
    {
        embed.LastItem.ExtraData = new();
        embed.LastItem.ExtraData["hascustomhaschanged"] = true;
        embed.LastItem.ExtraData["HasChanged"] = HasChanged;
        return embed;
    }

    internal static ulong ToHash(this EmbedItem item, SHA256 sha)
    {
        var children = item.Children;

        // gotta clear bc we should NOT update the parent item if a child cries (changes)
        item.Children = new();
        
        // for the number of children
        if (children is not null)
        {
            foreach (var _item in children)
            {
                item.Children.Add(new());
            }
        }
        string text = JsonSerializer.Serialize(item, options);
        byte[] buffer = Encoding.Unicode.GetBytes(text);
        item.Children = children;

        return BitConverter.ToUInt64(sha.ComputeHash(buffer));
    }

    public static EmbedBuilder SetId(this EmbedBuilder embed, string id)
    {
        embed.LastItem.Id = id;
        return embed;
    }

    public static EmbedBuilder CalculateHashOnItemsForSettingHasChanged(this EmbedBuilder embedBuilder)
    {
        using (SHA256 sha = SHA256.Create())
        {
            foreach (var page in embedBuilder.embed.Pages)
            {
                foreach (var item in page.GetAllItems())
                {
                    if (item.Id is not null)
                    {
                        if (item.ExtraData is null)
                            item.ExtraData = new();
                        item.ExtraData["hash"] = item.ToHash(sha);
                    }
                }
            }
        }
        return embedBuilder;
    }

    public static EmbedBuilder CalculateHasChangedForAllItems(this EmbedBuilder embedBuilder, ConcurrentDictionary<string, ulong>? previtems)
    {
        foreach (var page in embedBuilder.embed.Pages)
        {
            foreach (var item in page.GetAllItems()) 
            {
                if (previtems is null)
                {
                    if (false)
                    {
                        if (item.ExtraData is null)
                            item.ExtraData = new();
                        item.ExtraData["HasChanged"] = true;
                    }
                    embedBuilder.Data["sendfullupdate"] = true;
                    break;
                }
                else
                {
                    if (item.ExtraData is not null && item.ExtraData.ContainsKey("hascustomhaschanged"))
                        continue;
                    else if (item.ExtraData is null || !item.ExtraData.ContainsKey("hash") || item.Id == null)
                    {
                        if (item.ExtraData is null)
                            item.ExtraData = new();
                        item.ExtraData["HasChanged"] = false;
                    }
                    else if (previtems.ContainsKey(item.Id))
                    {
                        item.ExtraData["HasChanged"] = previtems[item.Id] != (ulong)item.ExtraData["hash"];
                    }
                    else
                    {
                        if (item.ExtraData is null)
                            item.ExtraData = new();
                        item.ExtraData["IsMostLikeyNew"] = true;
                        item.ExtraData["HasChanged"] = false;
                    }
                }
            }
        }
        return embedBuilder;
    }

    public static EmbedBuilder CalculateHasChangedForAllItems(this EmbedBuilder embedBuilder, Dictionary<string, ulong>? previtems)
    {
        foreach (var page in embedBuilder.embed.Pages)
        {
            foreach (var item in page.GetAllItems())
            {
                if (previtems is null)
                {
                    if (false)
                    {
                        if (item.ExtraData is null)
                            item.ExtraData = new();
                        item.ExtraData["HasChanged"] = true;
                    }
                    embedBuilder.Data["sendfullupdate"] = true;
                    break;
                }
                else
                {
                    if (item.ExtraData is not null && item.ExtraData.ContainsKey("hascustomhaschanged"))
                        continue;
                    else if (item.ExtraData is null || !item.ExtraData.ContainsKey("hash") || !item.ExtraData.ContainsKey("id"))
                    {
                        if (item.ExtraData is null)
                            item.ExtraData = new();
                        item.ExtraData["HasChanged"] = false;
                    }
                    else if (previtems.ContainsKey((string)item.ExtraData["id"]))
                    {
                        item.ExtraData["HasChanged"] = previtems[(string)item.ExtraData["id"]] != (ulong)item.ExtraData["hash"];
                    }
                    else
                        item.ExtraData["HasChanged"] = false;
                }
            }
        }
        return embedBuilder;
    }

    public static EmbedBuilder AddProgress(this EmbedBuilder embed, string id)
    {
        embed.AddProgress();
        embed.LastItem.ExtraData ??= new();
        embed.LastItem.ExtraData["id"] = id;
        return embed;
    }

    public static EmbedBuilder AddButton(this EmbedBuilder embed, string text, string id)
    {
        embed.AddButtonWithNoText();
        embed.LastItem.ExtraData ??= new();
        embed.LastItem.ExtraData["id"] = id;

        embed.AddText(text);
        embed.LastItem.ExtraData ??= new();
        embed.LastItem.ExtraData["id"] = $"{id}-innertext-valour.net";

        embed.Close();
        return embed;
    }
}
