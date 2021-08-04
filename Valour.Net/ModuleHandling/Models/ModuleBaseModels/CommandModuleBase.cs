using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.Global;
using Valour.Net.Models;

namespace Valour.Net.CommandHandling
{
    /// <summary>
    /// This module base is loaded by the command service but is not checked for registrable commands
    /// </summary>
    public abstract class CommandModuleBase : IModuleBase
    {
        public CommandContext ctx { get; set; }

        public async Task setContext(CommandContext ctx)
        {
            this.ctx = ctx;
        }

        public async Task ReplyAsync(string content)
        {
            await ValourClient.PostMessage(ctx.Channel.Id, ctx.Planet.Id, content);
        }

        public async Task ReplyWithMessagesAsync(int delay, List<string> data)
        {
            foreach (string content in data)
            {
                await ValourClient.PostMessage(ctx.Channel.Id, ctx.Planet.Id, content);
                await Task.Delay(delay);
            }
        }
    }
}
