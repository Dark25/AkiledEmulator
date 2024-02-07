using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using System.Linq;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class roomkick : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetHabbo().CurrentRoom;
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;
            string MessageAlert = CommandManager.MergeParams(Params, 1);
            if (Session.Antipub(MessageAlert, "<CMD>"))
            {
                AkiledEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("publicidad", string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("roomkick.pub.1", Session.Langue), Session.GetHabbo().Username, MessageAlert), "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
                return;
            }

            foreach (RoomUser User in currentRoom.GetRoomUserManager().GetUserList().ToList())
            {
                if (User != null && !User.IsBot && !User.GetClient().GetHabbo().HasFuse("fuse_no_kick"))
                {
                    if (MessageAlert.Length > 0) User.GetClient().SendNotification(MessageAlert);

                    User.AllowMoveTo = false;
                    User.IsWalking = true;
                    User.GoalX = Room.GetGameMap().Model.DoorX;
                    User.GoalY = Room.GetGameMap().Model.DoorY;

                    currentRoom.GetRoomUserManager().RemoveUserFromRoom(User.GetClient(), true, false);
                }
            }

        }
    }
}
