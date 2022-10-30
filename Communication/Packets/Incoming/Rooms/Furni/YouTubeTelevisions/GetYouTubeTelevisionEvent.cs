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

        bool UserHasRights;
        private static Item Item;

        public void Parse(GameClient Session, ClientPacket packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;


            if (Session.GetHabbo().SendWebPacket(new YoutubeTvComposer((UserHasRights) ? Item.Id : 0, Item.ExtraData)))
                return;

            if (!UserHasRights)
                return;

            RoomUser roomUser = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUser == null)
                return;
            if (string.IsNullOrEmpty(roomUser.LoaderVideoId) && string.IsNullOrEmpty(Item.ExtraData))
            {
                roomUser.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("item.tpyoutubehelp", Session.Langue));
            }

            if (!string.IsNullOrEmpty(roomUser.LoaderVideoId) && roomUser.LoaderVideoId != Item.ExtraData)
            {
                Item.ExtraData = roomUser.LoaderVideoId;
                Item.UpdateState();

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
