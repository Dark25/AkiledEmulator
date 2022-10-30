using Akiled.HabboHotel.GameClients;namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd{    class Emptyitems : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {
            bool EmptyAll = (Params.Length > 1 && Params[1] == "all");

            Session.GetHabbo().GetInventoryComponent().ClearItems(EmptyAll);            Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("empty.cleared", Session.Langue));        }    }}