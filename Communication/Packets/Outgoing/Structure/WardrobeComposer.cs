using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using System;
using System.Data;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class WardrobeComposer : ServerPacket
    {
        public WardrobeComposer(GameClient Session)
            : base(ServerPacketHeader.WardrobeMessageComposer)
        {
            WriteInteger(1);
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `slot_id`,`look`,`gender` FROM `user_wardrobe` WHERE `user_id` = '" + Session.GetHabbo().Id + "' LIMIT 10");
                DataTable WardrobeData = dbClient.GetTable();

                if (WardrobeData == null)
                    WriteInteger(0);
                else
                {
                    WriteInteger(WardrobeData.Rows.Count);
                    foreach (DataRow Row in WardrobeData.Rows)
                    {
                        WriteInteger(Convert.ToInt32(Row["slot_id"]));
                        WriteString(Convert.ToString(Row["look"]));
                        WriteString(Row["gender"].ToString().ToUpper());
                    }
                }
            }
        }
    }
}
