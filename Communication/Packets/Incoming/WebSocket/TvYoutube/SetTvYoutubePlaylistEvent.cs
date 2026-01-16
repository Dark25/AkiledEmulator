using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.WebClients;
using System.Text.RegularExpressions;

namespace Akiled.Communication.Packets.Incoming.WebSocket
{
    class SetTvYoutubePlaylistEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            string Payload = Packet.PopString();

            GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetHabbo() == null)
                return;

            Room room = Client.GetHabbo().CurrentRoom;
            if (room == null || !room.CheckRights(Client))
                return;

            Item item = room.GetRoomItemHandler().GetItem(ItemId);
            if (item == null || item.GetBaseItem().InteractionType != InteractionType.tvyoutube)
                return;

            // Try to extract a video id (11 chars) from payload
            var m = Regex.Match(Payload ?? "", "[A-Za-z0-9_-]{11}");
            if (!m.Success)
                return;

            string videoId = m.Value;

            item.ExtraData = videoId;
            item.UpdateState();

            room.SendPacketWeb(new Akiled.Communication.Packets.Outgoing.WebSocket.YoutubeTvComposer(ItemId, videoId));
        }
    }
}
