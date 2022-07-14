using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Quests;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Rooms.Engine
{
    internal class UseWallItemEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || !session.GetHabbo().InRoom)
                return;

            if (!AkiledEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            int itemId = packet.PopInt();
            Item item = room.GetRoomItemHandler().GetItem(itemId);
            if (item == null)
                return;
            bool UserHasRights = false;
            if (room.CheckRights(session))
                UserHasRights = true;

            int request = packet.PopInt();

            item.Interactor.OnTrigger(session, item, request, UserHasRights);

            AkiledEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.EXPLORE_FIND_ITEM, item.GetBaseItem().Id);
        }
    }
}

