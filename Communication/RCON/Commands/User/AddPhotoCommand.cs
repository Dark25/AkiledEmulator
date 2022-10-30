using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;

namespace Akiled.Communication.RCON.Commands.User
{
    class AddPhotoCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            if (parameters.Length != 3)
                return false;

            int Userid = 0;
            if (!int.TryParse(parameters[1], out Userid))
                return false;
            if (Userid == 0)
                return false;

            GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);
            if (Client == null)
                return false;

            string PhotoId = parameters[2];

            ItemData ItemData = null;
            if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(4581, out ItemData))
                return false;

            int Time = AkiledEnvironment.GetUnixTimestamp();
            string ExtraData = "{\"w\":\"" + "/photos/" + PhotoId + ".png" + "\", \"n\":\"" + Client.GetHabbo().Username + "\", \"s\":\"" + Client.GetHabbo().Id + "\", \"u\":\"" + "0" + "\", \"t\":\"" + Time + "000" + "\"}";

            Item Item = ItemFactory.CreateSingleItemNullable(ItemData, Client.GetHabbo(), ExtraData);
            Client.GetHabbo().GetInventoryComponent().TryAddItem(Item);

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("INSERT INTO user_photos (user_id,photo,time) VALUES ('" + Client.GetHabbo().Id + "', @photoid, '" + Time + "');");
                queryreactor.AddParameter("photoid", PhotoId);
                queryreactor.RunQuery();
            }

            Client.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.valide", Client.Langue));

            return true;
        }
    }
}
