using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;

namespace Akiled.Communication.Packets.Incoming.Rooms.Furni.Moodlight
{
    class MoodlightUpdateEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!AkiledEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out var room))
            {
                return;
            }

            if (!room.CheckRights(session, true) || room.MoodlightData == null)
            {
                return;
            }

            var roomItem = room.GetRoomItemHandler().GetItem(room.MoodlightData.ItemId);
            if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.MOODLIGHT)
            {
                return;
            }

            var preset = packet.PopInt();
            var num = packet.PopInt();
            var color = packet.PopString();
            var intensity = packet.PopInt();

            var bgOnly = false;

            if (num >= 2)
            {
                bgOnly = true;
            }

            room.MoodlightData.Enabled = true;
            room.MoodlightData.CurrentPreset = preset;
            room.MoodlightData.UpdatePreset(preset, color, intensity, bgOnly);
            roomItem.ExtraData = room.MoodlightData.GenerateExtraData();
            roomItem.UpdateState();
        }
    }
}