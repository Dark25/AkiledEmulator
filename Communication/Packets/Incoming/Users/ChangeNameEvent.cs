
using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    internal class ChangeNameEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null || Session == null)
                return;
            Room room1 = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room1 == null)
                return;
            RoomUser roomUserByHabbo = room1.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
            if (roomUserByHabbo == null)
                return;
            string str = Packet.PopString();
            if (!Session.GetHabbo().CanChangeName && Session.GetHabbo().Rank == 1)
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.changename.error.1", Session.Langue));
            else if (str == Session.GetHabbo().Username)
                Session.SendPacket((IServerPacket)new UpdateUsernameComposer(Session.GetHabbo().Username));
            else if (this.NameAvailable(str) != 1)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.changename.error.2", Session.Langue));
            }
            else
            {
                using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryReactor.SetQuery("UPDATE rooms SET owner = @newname WHERE owner = @oldname");
                    queryReactor.AddParameter("newname", str);
                    queryReactor.AddParameter("oldname", Session.GetHabbo().Username);
                    queryReactor.RunQuery();
                    queryReactor.SetQuery("UPDATE users SET username = @newname WHERE id = @userid");
                    queryReactor.AddParameter("newname", str);
                    queryReactor.AddParameter("userid", Session.GetHabbo().Id);
                    queryReactor.RunQuery();
                    queryReactor.SetQuery("INSERT INTO `logs_flagme` (`user_id`, `oldusername`, `newusername`, `time`) VALUES (@userid, @oldusername, @newusername, '" + AkiledEnvironment.GetUnixTimestamp().ToString() + "');");
                    queryReactor.AddParameter("userid", Session.GetHabbo().Id);
                    queryReactor.AddParameter("oldusername", Session.GetHabbo().Username);
                    queryReactor.AddParameter("newusername", str);
                    queryReactor.RunQuery();
                }
                AkiledEnvironment.GetGame().GetClientManager().UpdateClientUsername(Session.ConnectionID, Session.GetHabbo().Username, str);
                room1.GetRoomUserManager().UpdateClientUsername(roomUserByHabbo, Session.GetHabbo().Username, str);
                Session.GetHabbo().Username = str;
                Session.GetHabbo().CanChangeName = false;
                Session.SendPacket((IServerPacket)new UpdateUsernameComposer(str));
                Session.SendPacket((IServerPacket)new UserObjectComposer(Session.GetHabbo()));
                Session.GetHabbo().UpdateRooms();
                foreach (RoomData usersRoom in Session.GetHabbo().UsersRooms)
                {
                    usersRoom.OwnerName = str;
                    Room room2 = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(usersRoom.Id);
                    if (room2 != null)
                        room2.RoomData.OwnerName = str;
                }
                room1.SendPacket((IServerPacket)new UserNameChangeMessageComposer(0, roomUserByHabbo.VirtualId, str));
                if (Session.GetHabbo().Id != room1.RoomData.OwnerId)
                    return;
                room1.SendPacket((IServerPacket)new RoomInfoUpdatedMessageComposer(room1.Id));
            }
        }

        private int NameAvailable(string username)
        {
            username = username.ToLower();
            if (username.Length > 15 || username.Length < 3)
                return -2;
            return !AkiledEnvironment.IsValidAlphaNumeric(username) ? -1 : (AkiledEnvironment.UsernameExists(username) ? 0 : 1);
        }
    }
}
