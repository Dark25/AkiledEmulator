using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using System.Linq;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class roombadge : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetHabbo().CurrentRoom;
            if (currentRoom == null)
                return;

            string local_0 = (string)Params[1];
            foreach (RoomUser item_0 in currentRoom.GetRoomUserManager().GetUserList().ToList())
            {
                try
                {
                    if (!item_0.IsBot)
                    {
                        if (item_0.GetClient() != null)
                        {
                            if (item_0.GetClient().GetHabbo() != null)
                            {
                                item_0.GetClient().GetHabbo().GetBadgeComponent().GiveBadge(local_0, 0, true);
                                item_0.GetClient().SendPacket(new ReceiveBadgeComposer(local_0));
                                item_0.GetClient().SendPacket(new NewYearComposer(local_0));
                            }
                        }
                    }
                }
                catch
                {
                }
            }

        }
    }
}
