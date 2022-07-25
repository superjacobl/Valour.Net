using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Valour.Net.CommandHandling;

public class CommandContext
{
    public Planet Planet { get; set;}
    public PlanetChatChannel Channel { get; set;}
    public PlanetMember Member { get; set;}
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

    /// <summary>
    /// Sends a message in the same channel as the Command was executed in.
    /// </summary>
    /// <param name="content">What the message should say.</param>
    /// <returns></returns>

    public Task ReplyAsync(string content)
    {
        return ValourNetClient.PostMessage(Channel.Id, Planet.Id, content);
    }

    public Task ReplyAsync(EmbedBuilder embedbuilder)
    {
        return ValourNetClient.PostMessage(Channel.Id, Planet.Id, "", embedbuilder.embed);
    }

    /// <summary>
    /// The message will be sent after {delay}ms.
    /// </summary>
    /// <param name="delay">Time in ms to delay.</param>
    /// <param name="data">What the message should say.</param>
    /// <returns></returns>

    public async Task ReplyWithDelayAsync(int delay, string content) {
        await Task.Delay(delay);
        await ValourNetClient.PostMessage(Channel.Id, Planet.Id, content, null);
    }

    /// <summary>
    /// Each item in data will be delayed by delay. Ex: if data is ["Hello", "World"], and delay is 100ms, "Hello" will be sent, then 100ms will pass, then "World" would be sent.
    /// </summary>
    /// <param name="delay">Time in ms to delay each message for.</param>
    /// <param name="data">A list of strings to send as messages.</param>
    /// <returns></returns>
    public async Task ReplyWithMessagesAsync(int delay, List<string> data) {
        foreach (string content in data) {
            await ValourNetClient.PostMessage(Channel.Id, Planet.Id, content, null);
            await Task.Delay(delay);
        }
    }
}