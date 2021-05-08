using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace Valour.Net.Models
{
    public class Channel
    {
        /// <summary>
        /// The Id of this channel
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// The name of this channel
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Id of the planet this channel belongs to
        /// </summary>
        public ulong Planet_Id { get; set; }

        /// <summary>
        /// The amount of messages ever sent in the channel
        /// </summary>
        public ulong Message_Count { get; set; }

        /// <summary>
        /// The id of the parent category, is null if theres no parent
        /// </summary>
        public ulong? Parent_Id { get; set;}

        /// <summary>
        /// Is the position in the category/channel list
        /// </summary>
        public ushort Position { get; set; }

        /// <summary>
        /// The description of the channel
        /// </summary>
        public string Description { get; set; }

        public async Task SendMessage(string content)
        {
            await ValourClient.PostMessage(Id, Planet_Id, content);
        }

    }
}