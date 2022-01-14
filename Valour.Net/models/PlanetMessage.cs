using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.Models.Planets;

namespace Valour.Net.Models.Messages
{
    public class NetMessage : PlanetMessage
    {
        public NetChannel Channel { 
            get
            {
                return (NetChannel)((PlanetChatChannel.FindAsync(this.Channel_Id)).Result);
            } 
        }
        public PlanetMember Author
        {
            get
            {
                return (PlanetMember.FindAsync(Member_Id)).Result;
            }
        }
        public Planet Planet
        {
            get
            {
                return (Planet.FindAsync(Planet_Id)).Result;
            }
        }
        public NetMessage()
        {
            
        }
    }
}
