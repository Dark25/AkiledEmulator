﻿using Akiled.Database.Interfaces;
using System;
using System.Data;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    internal class MarketPlaceOwnOffersComposer : ServerPacket
    {
        public MarketPlaceOwnOffersComposer(int UserId)
           : base(ServerPacketHeader.MarketPlaceOwnOffersMessageComposer)
        {
            int i = 0;
            DataTable table = null;
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT timestamp, state, offer_id, item_type, sprite_id, total_price, limited_number, limited_stack FROM catalog_marketplace_offers WHERE user_id = '" + UserId + "'");
                table = dbClient.GetTable();

                dbClient.SetQuery("SELECT SUM(asking_price) FROM catalog_marketplace_offers WHERE state = '2' AND user_id = '" + UserId + "'");
                i = dbClient.GetInteger();

                WriteInteger(i);
                if (table != null)
                {
                    WriteInteger(table.Rows.Count);
                    foreach (DataRow row in table.Rows)
                    {
                        int num2 = Convert.ToInt32(Math.Floor((double)(((((double)row["timestamp"]) + 172800.0) - AkiledEnvironment.GetUnixTimestamp()) / 60.0)));
                        int num3 = int.Parse(row["state"].ToString());
                        if ((num2 <= 0) && (num3 != 2))
                        {
                            num3 = 3;
                            num2 = 0;
                        }
                        WriteInteger(Convert.ToInt32(row["offer_id"]));
                        WriteInteger(num3);
                        WriteInteger(1);
                        WriteInteger(Convert.ToInt32(row["sprite_id"]));

                        WriteInteger(256);
                        WriteString("");
                        WriteInteger(Convert.ToInt32(row["limited_number"]));
                        WriteInteger(Convert.ToInt32(row["limited_stack"]));

                        WriteInteger(Convert.ToInt32(row["total_price"]));
                        WriteInteger(num2);
                        WriteInteger(Convert.ToInt32(row["sprite_id"]));
                    }
                }
                else
                {
                    WriteInteger(0);
                }
            }
        }
    }
}