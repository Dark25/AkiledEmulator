using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetGiftWrappingConfigurationEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            ServerPacket Response = new ServerPacket(ServerPacketHeader.GiftWrappingConfigurationMessageComposer);
            Response.WriteBoolean(true);
            Response.WriteInteger(1);

            Response.WriteInteger(9);
            for (int i = 3372; i < 3381; ++i)
                Response.WriteInteger(i);

            Response.WriteInteger(8);
            Response.WriteInteger(0);
            Response.WriteInteger(1);
            Response.WriteInteger(2);
            Response.WriteInteger(3);
            Response.WriteInteger(4);
            Response.WriteInteger(5);
            Response.WriteInteger(6);
            Response.WriteInteger(8);
            Response.WriteInteger(11);
            Response.WriteInteger(0);
            Response.WriteInteger(1);
            Response.WriteInteger(2);
            Response.WriteInteger(3);
            Response.WriteInteger(4);
            Response.WriteInteger(5);
            Response.WriteInteger(6);
            Response.WriteInteger(7);
            Response.WriteInteger(8);
            Response.WriteInteger(9);
            Response.WriteInteger(10);

            Response.WriteInteger(7);
            for (int i = 187; i < 194; ++i)
                Response.WriteInteger(i);

            Session.SendPacket(Response);
        }
    }
}