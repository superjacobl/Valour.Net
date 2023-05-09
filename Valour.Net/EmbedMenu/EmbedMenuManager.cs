using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling;

namespace Valour.Net.EmbedMenu;

internal static class EmbedMenuManager
{
    internal static Dictionary<string, Func<InteractionContext, ValueTask>> ElementIdsToFuncs = new();

	public static async ValueTask ProcessInteraction(InteractionContext ctx)
    {
        if (ctx.Event.ElementId is null)
        {
            return;
        }
        if (ctx.Event.ElementId.Contains("::MENU$-"))
        {
            string id = ctx.Event.ElementId.Split("::MENU$-")[1];
            await ElementIdsToFuncs[$"MENU$-{id}"].Invoke(ctx);
        }
    }
}
