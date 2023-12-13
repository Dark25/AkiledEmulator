using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetModeratorUserRoomVisitsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {

            if (!Session.GetHabbo().HasFuse("fuse_helptool"))
                return;
            int num = Packet.PopInt();
            GameClient clientByUserId = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(num);
            if (clientByUserId == null)
                return;
            Dictionary<double, RoomData> Visits = new Dictionary<double, RoomData>();
            using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery("SELECT room_id, `entry_timestamp` FROM `user_roomvisits` WHERE `user_id` =@id ORDER BY `entry_timestamp` DESC LIMIT 50");
                queryReactor.AddParameter("id", num);
                DataTable table = queryReactor.GetTable();
                if (table != null)
                {
                    foreach (DataRow row in (InternalDataCollectionBase)table.Rows)
                    {
                        RoomData roomData = AkiledEnvironment.GetGame().GetRoomManager().GenerateRoomData(Convert.ToInt32(row["room_id"]));
                        if (roomData == null)
                            return;
                        if (!Visits.ContainsKey(Convert.ToDouble(row["entry_timestamp"])))
                            Visits.Add(Convert.ToDouble(row["entry_timestamp"]), roomData);
                    }
                }
            }
            Session.SendMessage((IServerPacket)new ModeratorUserRoomVisitsComposer(clientByUserId.GetHabbo(), Visits));
        }

    }
}

