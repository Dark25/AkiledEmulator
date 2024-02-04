using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using AkiledEmulator.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class RemoveAllRightsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
                return;

            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || !room.CheckRights(Session, true))
                return;

            if (room.UsersWithRights.Count == 0)
                return;

            foreach (int num in room.UsersWithRights)
            {
                RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(num);
                if (roomUserByHabbo != null)
                {
                    if (!roomUserByHabbo.IsBot)
                    {
                        roomUserByHabbo.SetStatus("flatctrl", "0");
                        roomUserByHabbo.UpdateNeeded = true;

                        roomUserByHabbo.GetClient().SendPacket(new YouAreControllerComposer(RoomRightLevels.NONE));
                    }
                }
                ServerPacket Response3 = new ServerPacket(ServerPacketHeader.FlatControllerRemovedMessageComposer);
                Response3.WriteInteger(room.Id);
                Response3.WriteInteger(num);
                Session.SendPacket(Response3);
            }
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery("DELETE FROM room_rights WHERE room_id = " + room.Id);

            room.UsersWithRights.Clear();

            ServerPacket Response2 = new ServerPacket(ServerPacketHeader.RoomRightsListMessageComposer);
            Response2.WriteInteger(room.RoomData.Id);
            Response2.WriteInteger(0);
            Session.SendPacket(Response2);

        }
    }
}
