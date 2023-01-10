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
            if (!session.GetHabbo().InRoom)
                return;

            if (!AkiledEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(session, true))
                return;

            if (room.MoodlightData == null)
            {
                foreach (Item item in room.GetRoomItemHandler().GetWall.ToList())
                {
                    if (item.GetBaseItem().InteractionType == InteractionType.MOODLIGHT)
                        room.MoodlightData = new MoodlightData(item.Id);
                }
            }

            if (room.MoodlightData == null)
                return;

            session.SendPacket(new MoodlightConfigComposer(room.MoodlightData));
        }
    }
}