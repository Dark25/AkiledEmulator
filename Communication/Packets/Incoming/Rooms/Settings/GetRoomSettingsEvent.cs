using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetRoomSettingsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room Room = AkiledEnvironment.GetGame().GetRoomManager().LoadRoom(Packet.PopInt());
            if (Room == null)
                return;

            if (!Room.CheckRights(Session, true) && !Session.GetHabbo().HasFuse("fuse_settings_room"))
                return;

            Session.SendPacket(new RoomSettingsDataComposer(Room.RoomData));
        }
    }
}