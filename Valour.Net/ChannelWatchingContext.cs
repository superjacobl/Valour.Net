using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Valour.Net.CommandHandling;

public class ChannelWatchingContext : IContext
{
    public ChannelWatchingUpdate channelWatchingUpdate { get; set; }

    public ChannelWatchingContext() { }

    internal async Task Set(ChannelWatchingUpdate update)
    {
        Channel = ValourCache.GetAll<PlanetChatChannel>().FirstOrDefault(x => x.Id == update.ChannelId);
        channelWatchingUpdate = update;
        Planet = await Planet.FindAsync(Channel.PlanetId);
    }
}