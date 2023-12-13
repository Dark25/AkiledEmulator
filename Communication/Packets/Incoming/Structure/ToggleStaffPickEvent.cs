using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Navigators;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    internal class ToggleStaffPickEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().HasFuse("fuse_room_staffpick"))
                return;
            Room Room = (Room)null;
            if (!AkiledEnvironment.GetGame().GetRoomManager().TryGetRoom(Packet.PopInt(), out Room) || Room == null)
                return;
            StaffPick room = (StaffPick)null;
            if (!AkiledEnvironment.GetGame().GetNavigator().TryGetStaffPickedRoom(Room.Id, out room))
            {
                if (AkiledEnvironment.GetGame().GetNavigator().TryAddStaffPickedRoom(Room.Id))
                {
                    using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        queryReactor.SetQuery("INSERT INTO `navigator_staff_picks` (`room_id`,`image`) VALUES (@roomId, null)");
                        queryReactor.AddParameter("roomId", Room.Id);
                        queryReactor.RunQuery();
                    }
                }
            }
            else if (AkiledEnvironment.GetGame().GetNavigator().TryRemoveStaffPickedRoom(Room.Id))
            {
                using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryReactor.SetQuery("DELETE FROM `navigator_staff_picks` WHERE `room_id` = @roomId LIMIT 1");
                    queryReactor.AddParameter("roomId", Room.Id);
                    queryReactor.RunQuery();
                }
            }
            ServerPacket Message = new ServerPacket(948);
            Message.WriteInteger(Room.Id);
            Room.SendPacket((IServerPacket)Message);
            Room.SendPacket((IServerPacket)new RoomInfoUpdatedMessageComposer(Room.Id));
        }
    }
}