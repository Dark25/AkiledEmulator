using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;


namespace Akiled.HabboHotel.HotelView
{
    public class HotelViewManager
    {
        public List<SmallPromo> HotelViewPromosIndexers = new List<SmallPromo>();

        public HotelViewManager()
        {
            this.InitHotelViewPromo();
        }

        public void InitHotelViewPromo()
        {
            HotelViewPromosIndexers.Clear();

            using (IQueryAdapter DbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DbClient.SetQuery("SELECT * from hotelview_promos WHERE hotelview_promos.enabled = '1' ORDER BY hotelview_promos.`index` ASC");
                DataTable dTable = DbClient.GetTable();

                foreach (DataRow dRow in dTable.Rows)
                {
                    HotelViewPromosIndexers.Add(new SmallPromo(Convert.ToInt32(dRow[0]), (string)dRow[1], (string)dRow[2], (string)dRow[3], Convert.ToInt32(dRow[4]), (string)dRow[5], (string)dRow[6]));
                }
            }
        }

        public ServerPacket SmallPromoComposer(ServerPacket Message)
        {
            Message.WriteInteger(HotelViewPromosIndexers.Count);
            foreach (SmallPromo promo in HotelViewPromosIndexers)
            {
                promo.Serialize(Message);
            }

            return Message;
        }

    }
}
