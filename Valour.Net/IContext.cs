using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Collections.Concurrent;
using Valour.Net;
using Valour.Api.Client;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Valour.Net.CommandHandling;

public abstract class IContext
{
    public Planet Planet { get; set;}
    public Channel Channel { get; set;}
    public PlanetMember Member { get; set;}

	public AsyncServiceScope ServiceScope { get; set; }

	/// <summary>
	/// Mainly used for EventFilters
	/// </summary>
	public Dictionary<string, object> Items { get; set; } = new();

    /// <summary>
    /// Sends a message in the same channel as the Command was executed in.
    /// </summary>
    /// <param name="content">What the message should say.</param>
    /// <returns></returns>
    public async Task ReplyAsync(string content)
    {
        await ValourNetClient.PostMessage(Channel.Id, Planet.Id, content, null);
    }

    /// <summary>
    /// Sends a message in the same channel as the Command was executed in.
    /// </summary>
    /// <param name="embedbuilder">The embed to send.</param>
    /// <returns></returns>

    public async Task ReplyAsync(EmbedBuilder embedbuilder)
    {
        await ValourNetClient.PostMessage(Channel.Id, Planet.Id, "", embedbuilder.embed);
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