using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class DiceOffEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;
            Item roomItem = room.GetRoomItemHandler().GetItem(Packet.PopInt());
            if (roomItem == null)
                return;
            bool UserHasRights = room.CheckRights(Session);
            roomItem.Interactor.OnTrigger(Session, roomItem, -1, UserHasRights);
            roomItem.OnTrigger(room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id));

        }
    }
}
