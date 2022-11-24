using System;
using System.Threading.Tasks;
using Valour.Net.CommandHandling;

namespace Valour.Net.EmbedMenu;

public static class EmbedMenuExtensions
{
    public static EmbedBuilder OnClick(this EmbedBuilder embed, Func<InteractionContext, ValueTask> onClick, string extraid = " ")
    {
        var item = embed.embed.GetLastItem(true);
        var name = $"MENU$-{onClick.Method.Module.Name}.{onClick.Method.Name}";
        switch (item.ItemType)
        {
            case EmbedItemType.Text:
                item.OnClickEventName = $"{extraid}::{name}";
                break;
            case EmbedItemType.Button:
                var _item = (EmbedButtonItem)item;
                _item.Id = $"{extraid}::{EmbedMenuManager.ElementIdsToFuncs.Count}";
                break;
        }
        //EmbedMenuManager.ElementIdsToFuncs[$"MENU$-{EmbedMenuManager.ElementIdsToFuncs.Count}"] = onClick;
        EmbedMenuManager.ElementIdsToFuncs[name] = onClick;
        return embed;
    }
}
