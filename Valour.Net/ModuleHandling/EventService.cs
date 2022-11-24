using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Valour.Net.CommandHandling.InfoModels;
using Valour.Net.ModuleHandling.Models.InfoModels;
using Valour.Api.Client;

namespace Valour.Net.CommandHandling;

public enum EventType
{
    Message,
    Interaction,
    AfterCommand,
    FriendRequest,
    OnChannelWatching
}

internal static class EventService
{
    public static List<EventInfo> _Events = new List<EventInfo>();
    public static List<InteractionEventInfo> _InteractionEvents = new List<InteractionEventInfo>();


    public static async Task UserLacksTheRolesToUseACommand(CommandInfo command, CommandContext ctx)
    {
        // change this to use something similar to how valour handles requests
        // ex: [HasRole("You must be mod or above to use this command!", "mod", "admin"]

        EventInfo Event = _Events.First(x => x.eventType == null);

        if (Event != null) {
            object[] args = new object[2];
            args[0] = ctx;
            args[1] = command.MainAlias;
            Event.Method.Invoke(Event.moduleInfo.Instance, args);
        }
    }

    internal static async Task OnChannelWatching(ChannelWatchingContext ctx)
    {
        foreach(EventInfo Event in _Events.Where(x => x.eventType == EventType.OnChannelWatching)) {
            object[] args = new object[1];
            args[0] = ctx;
            await ValourNetClient.InvokeMethod(Event.Method, Event.moduleInfo.Instance, args);
        }
    }

    internal static async Task OnMessage(CommandContext ctx)
    {
        foreach(EventInfo Event in _Events.Where(x => x.eventType == EventType.Message)) {
            object[] args = new object[1];
            args[0] = ctx;
            await ValourNetClient.InvokeMethod(Event.Method, Event.moduleInfo.Instance, args);
        }
    }

    internal static async Task AfterCommand(CommandContext ctx)
    {
        foreach (EventInfo Event in _Events.Where(x => x.eventType == EventType.AfterCommand))
        {
            object[] args = new object[1];
            args[0] = ctx;
            await ValourNetClient.InvokeMethod(Event.Method, Event.moduleInfo.Instance, args);
        }
    }

    internal static async Task ExecuteInteractionFunction(InteractionEventInfo Event, InteractionContext ctx) {
        object[] args = new object[1];
        args[0] = ctx;
        await ValourNetClient.InvokeMethod(Event.Method, Event.moduleInfo.Instance, args);
    }

    internal static async Task OnInteraction(EmbedInteractionEvent IEvent)
    {
        if (IEvent.Author_MemberId != (await ValourClient.GetSelfMember(IEvent.PlanetId)).Id) return;

        IEnumerable<InteractionEventInfo> eventInfos = _InteractionEvents.Where(x => x.EventType == IEvent.EventType);

        InteractionContext ctx = new();
        await ctx.SetFromImteractionEvent(IEvent);

        await EmbedMenuManager.ProcessInteraction(ctx);

        var infos = eventInfos;
        infos = infos.Where(x => x.InteractionElementId is null || (x.InteractionElementId is not null && x.InteractionElementId == IEvent.ElementId));
        infos = infos.Where(x => x.InteractionFormId is null || (x.InteractionFormId is not null && x.InteractionFormId == IEvent.FormId));

        foreach (InteractionEventInfo Event in infos)
        {
            await ExecuteInteractionFunction(Event, ctx);
        }
    }
}
