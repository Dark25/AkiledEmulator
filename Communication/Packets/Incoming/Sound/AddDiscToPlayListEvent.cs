using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Sound
{
    internal class AddDiscToPlayListEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room currentRoom = Session.GetHabbo().CurrentRoom;
            if (!currentRoom.CheckRights(Session))
                return;
            int pId = Packet.PopInt();
            Packet.PopInt();
            Item obj = currentRoom.GetRoomItemHandler().GetItem(pId);
            if (obj == null || currentRoom.GetTraxManager().AddDisc(obj))
                return;
            Session.SendMessage((IServerPacket)new RoomNotificationComposer("", "Oeps! Haal het oude item eerst uit de Jukebox.", "error", "", ""));
        }
    }
}
