using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Users;
using AkiledEmulator.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class AssignRightsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
                return;

            int UserId = Packet.PopInt();

            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;

            if (room == null || !room.CheckRights(Session, true))
                return;

            if (room.UsersWithRights.Contains(UserId))
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("user.giverights.error", Session.Langue));
            }
            else
            {
                Habbo Userright = AkiledEnvironment.GetHabboById(UserId);
                if (Userright == null)
                    return;

                room.UsersWithRights.Add(UserId);

                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    queryreactor.RunQuery("INSERT INTO room_rights (room_id,user_id) VALUES (" + room.Id + "," + UserId + ")");

                ServerPacket Response = new ServerPacket(ServerPacketHeader.FlatControllerAddedMessageComposer);
                Response.WriteInteger(room.Id);
                Response.WriteInteger(UserId);
                Response.WriteString(Userright.Username);
                Session.SendPacket(Response);

                RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(UserId);
                if (roomUserByHabbo == null || roomUserByHabbo.IsBot)
                    return;

                roomUserByHabbo.SetStatus("flatctrl", "1");
                roomUserByHabbo.UpdateNeeded = true;

                roomUserByHabbo.GetClient().SendPacket(new YouAreControllerComposer(RoomRightLevels.RIGHTS));
            }
        }
    }
}