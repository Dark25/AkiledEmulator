using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetMoodlightConfigEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || !room.CheckRights(Session, true))
                return;
            if (room.MoodlightData == null || room.MoodlightData.Presets == null)
                return;
            ServerPacket Response = new ServerPacket(ServerPacketHeader.MoodlightConfigMessageComposer);
            Response.WriteInteger(room.MoodlightData.Presets.Count);
            Response.WriteInteger(room.MoodlightData.CurrentPreset);
            int i = 0;
            foreach (MoodlightPreset moodlightPreset in room.MoodlightData.Presets)
            {
                ++i;
                Response.WriteInteger(i);
                Response.WriteInteger(int.Parse(AkiledEnvironment.BoolToEnum(moodlightPreset.BackgroundOnly)) + 1);
                Response.WriteString(moodlightPreset.ColorCode);
                Response.WriteInteger(moodlightPreset.ColorIntensity);
            }
            Session.SendPacket(Response);
        }
    }
}