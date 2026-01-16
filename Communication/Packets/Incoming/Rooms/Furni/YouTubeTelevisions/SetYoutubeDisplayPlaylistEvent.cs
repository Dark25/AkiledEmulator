using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;
using System.Text.RegularExpressions;

namespace Akiled.Communication.Packets.Incoming.Rooms.Furni.YouTubeTelevisions
{
    internal class SetYoutubeDisplayPlaylistEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            int ItemId = packet.PopInt();
            string payload = packet.PopString();

            Room room = Session.GetHabbo().CurrentRoom;
            if (room == null || !room.CheckRights(Session))
                return;

            var item = room.GetRoomItemHandler().GetItem(ItemId);
            if (item == null || item.GetBaseItem().InteractionType != InteractionType.tvyoutube)
                return;

            var m = Regex.Match(payload ?? "", "[A-Za-z0-9_-]{11}");
            if (!m.Success)
                return;

            string videoId = m.Value;

            item.ExtraData = videoId;
            item.UpdateState();

            // Broadcast to web clients (legacy) as well
            room.SendPacketWeb(new Akiled.Communication.Packets.Outgoing.WebSocket.YoutubeTvComposer(ItemId, videoId));
        }
    }
}
