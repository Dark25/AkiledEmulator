using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;
using System.Linq;

namespace Akiled.Communication.Packets.Incoming.Rooms.Furni.Moodlight
{
    class GetMoodlightConfigEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
             if (!AkiledEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
        {
            return;
        }

        if (room.MoodlightData == null || room.MoodlightData.Presets == null)
        {
            return;
        }

        session.SendPacket(new MoodlightConfigComposer(room.MoodlightData));
        }
    }
}