using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Rooms.Furni.Moodlight
{
    class MoodlightUpdateEvent : IPacketEvent
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

            int preset = packet.PopInt();
            int backgroundMode = packet.PopInt();
            string colorCode = packet.PopString();
            int intensity = packet.PopInt();

            bool backgroundOnly = backgroundMode >= 2;

            room.MoodlightData.Enabled = true;
            room.MoodlightData.CurrentPreset = preset;
            room.MoodlightData.UpdatePreset(preset, colorCode, intensity, backgroundOnly);

            item.ExtraData = room.MoodlightData.GenerateExtraData();
            item.UpdateState();
        }
    }
}