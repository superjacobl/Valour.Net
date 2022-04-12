using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling.InfoModels;
using Valour.Net.Models.Embed;
using Valour.Net.ModuleHandling.Models.InfoModels;
using Valour.Api.Items.Users;
using Valour.Api.Items.Planets;
using Valour.Api.Client;

namespace Valour.Net.CommandHandling
{
    public static class EventService
    {
        public static List<EventInfo> _Events = new List<EventInfo>();
        public static List<InteractionEventInfo> _InteractionEvents = new List<InteractionEventInfo>();


        public static async Task UserLacksTheRolesToUseACommand(CommandInfo command, CommandContext ctx)
        {
            EventInfo Event = _Events.First(x => x.EventName == "User Lacks the Role To Use This Command");

            if (Event != null) {
                object[] args = new object[2];
                args[0] = ctx;
                args[1] = command.MainAlias;
                Event.Method.Invoke(Event.moduleInfo.Instance, args);
            }
        }

        public static async Task OnMessage(CommandContext ctx)
        {
            foreach(EventInfo Event in _Events.Where(x => x.EventName == "Message")) {
                object[] args = new object[1];
                args[0] = ctx;
                Task result = (Task)Event.Method.Invoke(Event.moduleInfo.Instance, args);
                await result;
            }
        }

        public static async Task OnInteraction(EmbedInteractionEvent IEvent)
        {
            if (IEvent.Author_Member_Id != (await ValourClient.GetSelfMember(IEvent.Planet_Id)).Id) return;
            if (_InteractionEvents.Any(x => x.InteractionID == IEvent.Element_Id))
            {
                foreach (InteractionEventInfo Event in _InteractionEvents.Where(x => x.InteractionName == IEvent.Event && x.InteractionID == IEvent.Element_Id))
                {
                    object[] args = new object[1];
                    InteractionContext ctx = new();
                    await ctx.SetFromImteractionEvent(IEvent);
                    args[0] = ctx;
                    Task result = (Task)Event.Method.Invoke(Event.moduleInfo.Instance, args);
                    await result;
                }
            }
            else
            {
                foreach (InteractionEventInfo Event in _InteractionEvents.Where(x => x.InteractionName == IEvent.Event && x.InteractionID == ""))
                {
                    object[] args = new object[1];
                    InteractionContext ctx = new();
                    await ctx.SetFromImteractionEvent(IEvent);
                    args[0] = ctx;
                    Task result = (Task)Event.Method.Invoke(Event.moduleInfo.Instance, args);
                    await result;
                }
            }
            foreach (InteractionEventInfo Event in _InteractionEvents.Where(x => x.InteractionName == "" && x.InteractionID == ""))
                {
                    object[] args = new object[1];
                    InteractionContext ctx = new();
                    await ctx.SetFromImteractionEvent(IEvent);
                    args[0] = ctx;
                    Task result = (Task)Event.Method.Invoke(Event.moduleInfo.Instance, args);
                    await result;
                }

            
        }

    }
}
