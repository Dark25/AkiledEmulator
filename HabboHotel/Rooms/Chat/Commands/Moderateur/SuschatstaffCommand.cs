using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Moderator

{
    class SuschatstaffCommand : IChatCommand
    {

        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            if (Params.Length == 1)
            {
                Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("SuschatstaffCommand", Session.Langue));
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            AkiledEnvironment.GetGame().GetClientManager().StaffWhisper("[Staff Alert] " + Message + "" + " - " + Session.GetHabbo().Username, 23);
            return;
        }

    }
}

