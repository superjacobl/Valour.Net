using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling;

namespace Valour.Net.Global
{
    /// <summary>
    /// The base interface for all module bases
    /// </summary>
    public interface IModuleBase
    {
        public Task ReplyAsync(string message);

        public Task ReplyWithMessagesAsync(int delay, List<string> data);
    }
}
