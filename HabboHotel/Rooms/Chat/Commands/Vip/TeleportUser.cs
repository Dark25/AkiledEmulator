using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class TeleportUser : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session.GetHabbo().CurrentRoom == null)
                return;
            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("TeleportUser.error", Session.Langue), 34);
            }
            else
            {
                RoomUser roomUserByHabboId = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId).GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                if (roomUserByHabboId == null)
                    return;
                roomUserByHabboId.TeleportEnabled = !roomUserByHabboId.TeleportEnabled;
                if (roomUserByHabboId.TeleportEnabled)
                    Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("TeleportUser.enabled", Session.Langue), 34);
                else
                    Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("TeleportUser.disabled", Session.Langue), 34);
            }
        }
    }
}
