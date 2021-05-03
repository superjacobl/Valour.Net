using System;
using System.Linq;
using System.Threading.Tasks;

namespace Valour.Net.Models
{
    public class PlanetMessage
    {
        /// <summary>
        /// The Id of the message
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// The user's ID
        /// </summary>
        public ulong Author_Id { get; set; }

        /// <summary>
        /// String representation of message
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The time the message was sent (in UTC)
        /// </summary>
        public DateTime TimeSent { get; set; }

        /// <summary>
        /// Id of the channel this message belonged to
        /// </summary>
        public ulong Channel_Id { get; set; }

        /// <summary>
        /// Index of the message
        /// </summary>
        public ulong Message_Index { get; set; }

        public ulong Planet_Id { get; set; }

        public async Task<PlanetMember> GetAuthorAsync()
        {
            return await Cache.GetPlanetMember(Author_Id, Planet_Id);
        }

        public async Task<Channel> GetChannelAsync()
        {
            return await Cache.GetPlanetChannel(Channel_Id, Planet_Id);
        }

        public async Task<Planet> GetPlanetAsync()
        {
            return await Cache.GetPlanet(Planet_Id);
        }
    }
}
