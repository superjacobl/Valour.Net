using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Valour.Api.Client;

namespace Valour.Net.Models.Planets
{
    public class NetChannel : PlanetChatChannel
    {
        public async Task SendMessageAsync(string content)
        {
            await ValourNetClient.PostMessage(Id, Planet_Id, content, null);
        }

        public async Task SendMessageAsync(EmbedBuilder embed)
        {
            await ValourNetClient.PostMessage(Id, Planet_Id, "", embed.Generate());
        }

    }
}