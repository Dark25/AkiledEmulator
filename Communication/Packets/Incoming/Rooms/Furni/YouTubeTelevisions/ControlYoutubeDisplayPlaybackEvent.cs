using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Rooms.Furni.YouTubeTelevisions
{
    internal class ControlYoutubeDisplayPlaybackEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            int ItemId = packet.PopInt();
            int action = packet.PopInt();

            Room room = Session.GetHabbo().CurrentRoom;
            if (room == null || !room.CheckRights(Session))
                return;

            var item = room.GetRoomItemHandler().GetItem(ItemId);
            if (item == null || item.GetBaseItem().InteractionType != InteractionType.tvyoutube)
                return;

            // action can be interpreted by clients; here we simply re-broadcast current video so clients handle control locally
            room.SendPacketWeb(new Akiled.Communication.Packets.Outgoing.WebSocket.YoutubeTvComposer(ItemId, item.ExtraData));
        }
    }
}
