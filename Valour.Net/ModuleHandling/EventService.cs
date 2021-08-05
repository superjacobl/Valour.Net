using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling.InfoModels;
using Valour.Net.Models;

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

        public static async Task OnInteraction(InteractionEvent IEvent)
        {
            if (IEvent.Author_Member_Id != (await Cache.GetPlanetMember(ValourClient.BotId, IEvent.Planet_Id)).Id) return;
            if (_InteractionEvents.Any(x => x.InteractionID == IEvent.Element_Id))
            {
                foreach (InteractionEventInfo Event in _InteractionEvents.Where(x => x.InteractionName == IEvent.Event && x.InteractionID == IEvent.Element_Id))
                {
                    object[] args = new object[2];
                    args[0] = IEvent;
                    CommandContext ctx = new();
                    await ctx.SetFromImteractionEvent(IEvent);
                    args[1] = ctx;
                    Task result = (Task)Event.Method.Invoke(Event.moduleInfo.Instance, args);
                    await result;
                }
            }else
            {
                foreach (InteractionEventInfo Event in _InteractionEvents.Where(x => x.InteractionName == IEvent.Event && x.InteractionID == null))
                {
                    object[] args = new object[2];
                    args[0] = IEvent;
                    CommandContext ctx = new();
                    await ctx.SetFromImteractionEvent(IEvent);
                    args[1] = ctx;
                    Task result = (Task)Event.Method.Invoke(Event.moduleInfo.Instance, args);
                    await result;
                }
            }

            
        }

    }
}
