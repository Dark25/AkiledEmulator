using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class AddPhoto : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
                return;

            string PhotoId = Params[1];
            int ItemPhotoId = 4581;
            ItemData ItemData = null;
            if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(ItemPhotoId, out ItemData))
                return;

            int Time = AkiledEnvironment.GetUnixTimestamp();
            string ExtraData = "{\"w\":\"" + "/photos/" + PhotoId + ".png" + "\", \"n\":\"" + Session.GetHabbo().Username + "\", \"s\":\"" + Session.GetHabbo().Id + "\", \"u\":\"" + "0" + "\", \"t\":\"" + Time + "000" + "\"}";

            Item Item = ItemFactory.CreateSingleItemNullable(ItemData, Session.GetHabbo(), ExtraData);
            Session.GetHabbo().GetInventoryComponent().TryAddItem(Item);
            //Session.SendPacket(new FurniListUpdateComposer());

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("INSERT INTO user_photos (user_id,photo,time) VALUES ('" + Session.GetHabbo().Id + "', @photoid, '" + Time + "');");
                queryreactor.AddParameter("photoid", PhotoId);
                queryreactor.RunQuery();
            }

            Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.valide", Session.Langue));
        }
    }
}
