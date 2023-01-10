using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using System;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.SpecialPvP
{

    class ConvertDucketsCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            int TotalValue = 0;

            try
            {
                DataTable Table = null;
                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT `id` FROM `items` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND (`room_id`=  '0' OR `room_id` = '')");
                    Table = dbClient.GetTable();
                }

                if (Table == null)
                {
                    UserRoom.SendWhisperChat("De momento usted no tiene monedas en su inventario!");
                    return;
                }

                foreach (DataRow Row in Table.Rows)
                {
                    Item Item = Session.GetHabbo().GetInventoryComponent().GetItem(Convert.ToInt32(Row[0]));
                    if (Item == null)
                        continue;

                    if (!Item.GetBaseItem().ItemName.StartsWith("CF_duckets_") && !Item.GetBaseItem().ItemName.StartsWith("CFC_duckets_"))
                        continue;

                    if (Item.RoomId > 0)
                        continue;

                    string[] Split = Item.GetBaseItem().ItemName.Split('_');
                    int Value = int.Parse(Split[1]);

                    using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    }

                    Session.GetHabbo().GetInventoryComponent().RemoveItem(Item.Id);

                    TotalValue += Value;

                    if (Value > 0)
                    {
                        Session.GetHabbo().Duckets += Value;
                        Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Value));
                        Session.GetHabbo().UpdateActivityPointsBalance();

                    }
                }

                if (TotalValue > 0)
                    Session.SendNotification("Todos sus duckets en inventario se llevaron a su monedero con un !\r\r(Total de: " + TotalValue + " duckets!");
                else
                    Session.SendNotification("Al parecer no tiene ningun otro articulo intercambiable!");
            }
            catch
            {
                Session.SendNotification("Oops, ocurrio un error mientras se intercambiaban sus duckets, contacte un administrador!");
            }
        }
    }
}
