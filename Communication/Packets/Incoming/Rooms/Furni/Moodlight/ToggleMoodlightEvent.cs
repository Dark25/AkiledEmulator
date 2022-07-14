using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Items;

namespace Akiled.Communication.Packets.Incoming.Rooms.Furni.Moodlight
{
    class ToggleMoodlightEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            if (!AkiledEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(session, true) || room.MoodlightData == null)
                return;

            Item item = room.GetRoomItemHandler().GetItem(room.MoodlightData.ItemId);
            if (item == null || item.GetBaseItem().InteractionType != InteractionType.MOODLIGHT)
                return;

            if (room.MoodlightData.Enabled)
                room.MoodlightData.Disable();
            else
                room.MoodlightData.Enable();

            item.ExtraData = room.MoodlightData.GenerateExtraData();
            item.UpdateState();
        }
    }
}