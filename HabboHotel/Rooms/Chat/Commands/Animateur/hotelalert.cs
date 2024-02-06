using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class hotelalert : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor escribe el mensaje a enviar");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            string hotelalert_alert = (AkiledEnvironment.GetConfig().data["hotelalert_alert"]);
            string AlertMessage = string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("hotelalert.1", Session.Langue), Session.GetHabbo().Username, Message);


            AkiledEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer(hotelalert_alert, AkiledEnvironment.GetLanguageManager().TryGetValue("StaffAlertTitle.1", Session.Langue), AlertMessage, AkiledEnvironment.GetLanguageManager().TryGetValue("StaffAlertButton.1", Session.Langue), 0, ""), Session.Langue);


            AkiledEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, 0, string.Empty, "staffalert", string.Format("Staff Alert: {0}", AlertMessage));

            return;



        }
    }
}
