using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.Communication.Packets.Incoming.Rooms.Furni.YouTubeTelevisions
{
    internal class GetYouTubeTelevisionEvent : IPacketEvent
    {

        public void Parse(GameClient Session, ClientPacket packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            int ItemId = packet.PopInt();

            Room room = Session.GetHabbo().CurrentRoom;
            if (room == null)
                return;

            var item = room.GetRoomItemHandler().GetItem(ItemId);
            if (item == null || item.GetBaseItem().InteractionType != InteractionType.tvyoutube)
                return;

            bool userHasRights = room.CheckRights(Session);

            var composer = new Akiled.Communication.Packets.Outgoing.WebSocket.YoutubeTvComposer((userHasRights) ? item.Id : 0, item.ExtraData);

            // Send to the requesting game client so Nitro clients open the widget
            try
            {
                Session.SendPacket(composer);
            }
            catch { }

            // Also broadcast to web clients in the room
            room.SendPacketWeb(composer);

            if (!userHasRights)
                return;

            RoomUser roomUser = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUser == null)
                return;

            if (string.IsNullOrEmpty(roomUser.LoaderVideoId) && string.IsNullOrEmpty(item.ExtraData))
            {
                roomUser.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("item.tpyoutubehelp", Session.Langue));
            }

            if (!string.IsNullOrEmpty(roomUser.LoaderVideoId) && roomUser.LoaderVideoId != item.ExtraData)
            {
                item.ExtraData = roomUser.LoaderVideoId;
                item.UpdateState();

                roomUser.LoaderVideoId = "";
            }
        }

        private static IEnumerable<TValue> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict)
        {
            Random rand = new();
            List<TValue> values = dict.Values.ToList();
            int size = dict.Count;
            while (true)
            {
                yield return values[rand.Next(size)];
            }
        }
    }
}
