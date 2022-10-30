using Akiled.Communication.Packets.Outgoing.Structure;using Akiled.Database.Interfaces;using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;namespace Akiled.Communication.Packets.Incoming.Structure{    class UpdateNavigatorSettingsEvent : IPacketEvent    {




        public void Parse(GameClient Session, ClientPacket Packet)        {
            int RoomId = Packet.PopInt();
            RoomData roomData = AkiledEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);            if (RoomId != 0 && (roomData == null || roomData.OwnerName.ToLower() != Session.GetHabbo().Username.ToLower()))                return;            Session.GetHabbo().HomeRoom = RoomId;            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())                queryreactor.RunQuery(string.Concat(new object[4]                {                   "UPDATE users SET home_room = ",                   RoomId,                   " WHERE id = ",                   Session.GetHabbo().Id                }));


            Session.SendPacket(new UserHomeRoomComposer(RoomId, 0));
        }
    }
}



