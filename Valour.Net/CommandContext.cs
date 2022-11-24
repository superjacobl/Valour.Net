using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Valour.Net.CommandHandling;

public class CommandContext : IContext
{
    public PlanetMessage Message { get; set;}
    public TimeSpan MessageTimeTook { get; set; }
    public DateTime TimeReceived { get; set; }

    /// <summary>
    /// The datetime of when Valour.Net started executing the command's method.
    /// </summary>
    public DateTime CommandStarted { get; set; }

    public string? Command { get; set;}
    
    public string? Group { get; set; }

    public CommandContext() { }

    internal async Task Set(PlanetMessage msg)
    {
        Channel = await PlanetChatChannel.FindAsync(msg.ChannelId, msg.PlanetId);
        Member = await PlanetMember.FindAsync(msg.AuthorMemberId, msg.PlanetId);
        Message = msg;
        Planet = await Planet.FindAsync(Channel.PlanetId);
    }
}