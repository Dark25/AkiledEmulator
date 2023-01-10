using Akiled.Database.Interfaces;
using System;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class CheckItemCommand : IChatCommand
    {
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session == null || Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Params[1].ToString().ToLower());
            if (User == null)
            {
                Session.SendWhisper("Por favor escribe el nombre del usuario.");
                return;
            }

            if (Params.Length == 1)

            {
                Session.SendWhisper("Por favor introduce el ID del Item.");
                return;
            }

            if (Params.Length >= 2)
            {
                int ItemId2;
                if (!int.TryParse(Convert.ToString(Params[2]), out ItemId2))
                {

                    Session.SendWhisper("Por favor, introduzca una ID de Item válida.", 1);
                    return;
                }
                int count3 = 0;
                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT COUNT(*) FROM `items` WHERE `base_item` = @item_id AND `user_id` = @uid LIMIT 1");
                    dbClient.AddParameter("item_id", ItemId2);
                    dbClient.AddParameter("uid", User.GetClient().GetHabbo().Id);
                    count3 = dbClient.GetInteger();
                }
                Session.SendWhisper("[ITEM-CHECK] " + User.GetClient().GetHabbo().Username + " Tiene la cantidad de " + count3 + " en su poder.", 1);
            }
            else if (Params.Length < 2)
            {
                int ItemId;
                if (!int.TryParse(Convert.ToString(Params[1]), out ItemId))
                {
                    Session.SendWhisper("Por favor, introduzca una ID de Item válida.", 1);
                    return;
                }
                int count2 = 0;
                using (IQueryAdapter dClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dClient.SetQuery("SELECT COUNT(*) FROM items WHERE i.base_item = @IID AS i INNER JOIN users AS u ON i.user_id = u.id WHERE u.rank < 5`");
                    dClient.AddParameter("IID", ItemId);
                    count2 = dClient.GetInteger();
                }
                Session.SendWhisper("[ITEM-CHECK] Hay " + count2 + " unidad(es) en circulación. (Han sido excluidos los staff menores a rango 5)", 1);
            }
            else
            {
                Session.SendWhisper("Usa el comando :checkitem <username> <itemID> ó :checkitem <itemID>.", 1);
            }
        }
    }
}
