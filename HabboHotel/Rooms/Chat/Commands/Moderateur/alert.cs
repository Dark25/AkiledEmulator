using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class alert : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 3)
                return;

            GameClient clientByUsername = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (clientByUsername == null)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else
            {
                string message = CommandManager.MergeParams(Params, 2);
                if (Session.Antipub(message, "<CMD>"))
                {
                    AkiledEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("publicidad", string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("alert.1", Session.Langue), Session.GetHabbo().Username, message), "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
                    return;
                }

                clientByUsername.SendNotification(message);
            }

        }
    }
}
