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
        Channel = ValourCache.GetAll<Channel>().FirstOrDefault(x => x.Id == update.ChannelId);
        if (Channel is not null)
        {
            channelWatchingUpdate = update;
            Planet = await Planet.FindAsync((long)Channel.PlanetId);
        }
    }
}