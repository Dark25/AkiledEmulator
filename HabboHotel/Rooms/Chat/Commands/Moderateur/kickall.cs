using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms.RoomIvokedItems;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class kickall : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetHabbo().CurrentRoom;
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;
            RoomKick kick = new RoomKick("Usted ha sido expulsado de la sala.", Session.GetHabbo().Id);
            List<RoomUser> local_1 = new List<RoomUser>();
            foreach (RoomUser user in room.GetRoomUserManager().GetUserList().ToList())
            {
                if (!user.IsBot && !user.GetClient().GetHabbo().HasFuse("fuse_no_kick") && kick.SaufId != user.GetClient().GetHabbo().Id)
                {
                    if (kick.Alert.Length > 0)
                        user.GetClient().SendNotification(kick.Alert);
                    local_1.Add(user);
                }
            }
            foreach (RoomUser item_1 in local_1)
            {
                if (item_1 == null || item_1.GetClient() == null)
                    continue;
                room.GetRoomUserManager().RemoveUserFromRoom(item_1.GetClient(), true, false);
            }

        }
    }
}
