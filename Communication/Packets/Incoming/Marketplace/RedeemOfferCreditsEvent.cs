using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using System;
using System.Data;

namespace Akiled.Communication.Packets.Incoming.Marketplace
{
    class RedeemOfferCreditsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int CreditsOwed = 0;

            DataTable Table = null;
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `asking_price` FROM `catalog_marketplace_offers` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `state` = '2'");
                Table = dbClient.GetTable();
            }

            if (Table != null)
            {
                foreach (DataRow row in Table.Rows)
                {
                    CreditsOwed += Convert.ToInt32(row["asking_price"]);
                }

                if (CreditsOwed >= 1)
                {
                    Session.GetHabbo().AkiledPoints += CreditsOwed;
                    Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, 0, 105));

                    using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("DELETE FROM `catalog_marketplace_offers` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `state` = '2'");
                        dbClient.RunQuery("UPDATE users SET vip_points = vip_points + " + CreditsOwed + " WHERE id = " + Session.GetHabbo().Id);
                    }
                }
            }
        }
    }
}