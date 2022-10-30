using Akiled.HabboHotel.Users.Inventory.Bots;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class BotInventoryComposer : ServerPacket
    {
        public BotInventoryComposer(ICollection<Bot> Bots)
           : base(ServerPacketHeader.BotInventoryMessageComposer)
        {
            WriteInteger(Bots.Count);
            foreach (Bot Bot in Bots.ToList())
            {
                WriteInteger(Bot.Id);
                WriteString(Bot.Name);
                WriteString(Bot.Motto);
                WriteString(Bot.Gender);
                WriteString(Bot.Figure);
            }
        }
    }
}
