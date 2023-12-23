using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Valour.Net.CommandHandling;

public class CommandContext : IContext
{
    public Message Message { get; set;}
    public TimeSpan MessageTimeTook { get; set; }
    public DateTime TimeReceived { get; set; }

    /// <summary>
    /// The datetime of when Valour.Net started executing the command's method.
    /// </summary>
    public DateTime CommandStarted { get; set; }

    public string? Command { get; set;}
    
    public string? Group { get; set; }

    public CommandContext() { }

    internal async Task Set(Message msg)
    {
        Channel = await Channel.FindAsync(msg.ChannelId, msg.PlanetId);
        Member = await PlanetMember.FindAsync((long)msg.AuthorMemberId, (long)msg.PlanetId);
        Message = msg;
        Planet = await Planet.FindAsync((long)Channel.PlanetId);
    }
}