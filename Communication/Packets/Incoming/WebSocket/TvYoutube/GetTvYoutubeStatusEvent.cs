using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.WebClients;
using System;

namespace Akiled.Communication.Packets.Incoming.WebSocket
{
    class GetTvYoutubeStatusEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();

            GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetHabbo() == null)
                return;

            Room room = Client.GetHabbo().CurrentRoom;
            if (room == null)
                return;

            Item item = room.GetRoomItemHandler().GetItem(ItemId);
            if (item == null || item.GetBaseItem().InteractionType != InteractionType.tvyoutube)
                return;

            // Send current video id/state back to web clients in room
            room.SendPacketWeb(new Akiled.Communication.Packets.Outgoing.WebSocket.YoutubeTvComposer(ItemId, item.ExtraData));
        }
    }
}
