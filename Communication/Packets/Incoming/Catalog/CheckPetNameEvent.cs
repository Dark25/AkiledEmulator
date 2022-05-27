using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.Catalog.Utilities;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class CheckPetNameEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string str = Packet.PopString();

            ServerPacket Response = new ServerPacket(ServerPacketHeader.CheckPetNameMessageComposer);
            Response.WriteInteger(PetUtility.CheckPetName(str) ? 0 : 2);
            Response.WriteString(str);
            Session.SendPacket(Response);
        }
    }
}