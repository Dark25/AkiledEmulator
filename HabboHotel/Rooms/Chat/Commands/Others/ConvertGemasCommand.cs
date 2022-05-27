using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
namespace Akiled.HabboHotel.Rooms.Chat.Commands.SpecialPvP
{
    class ConvertGemasCommand : IChatCommand
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

                    if (!Item.GetBaseItem().ItemName.StartsWith("MODE_") && !Item.GetBaseItem().ItemName.StartsWith("TEMP_"))
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
                        Session.GetHabbo().AkiledPoints += Value;
                        Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, Value, 105));
                        Session.GetHabbo().UpdateDiamondsBalance();

                        using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                            queryreactor.RunQuery("UPDATE users SET vip_points = vip_points + " + Value + " WHERE id = " + Session.GetHabbo().Id);
                    }
                }

                if (TotalValue > 0)
                    Session.SendNotification("Todas sus monedas de temporadas en inventario se llevaron a su monedero con un !\r\r(Total de: " + TotalValue + " monedas de temporada!");
                else
                    Session.SendNotification("Al parecer no tiene ningun otro articulo intercambiable!");
            }
            catch
            {
                Session.SendNotification("Oops, ocurrio un error mientras se intercambiaban sus monedas de temporada, contacte un administrador!");
            }
        }
    }
}
