using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.WebClients;
using System;

namespace Akiled.Communication.Packets.Incoming.WebSocket
{
    class EditTvYoutubeEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            string Url = Packet.PopString();

            GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetHabbo() == null)
                return;

            Room room = Client.GetHabbo().CurrentRoom;
            if (room == null || !room.CheckRights(Client))
                return;

            Item item = room.GetRoomItemHandler().GetItem(ItemId);
            if (item == null || item.GetBaseItem().InteractionType != InteractionType.tvyoutube)
                return;

            if (string.IsNullOrEmpty(Url) || (!Url.Contains("?v=") && !Url.Contains("youtu.be/"))) //https://youtu.be/_mNig3ZxYbM
            {                return;            }            string Split = "";            if (Url.Contains("?v="))            {                Split = Url.Split(new string[] { "?v=" }, StringSplitOptions.None)[1];            }            else if (Url.Contains("youtu.be/"))            {                Split = Url.Split(new string[] { "youtu.be/" }, StringSplitOptions.None)[1];            }            if (Split.Length < 11)            {                return;            }            string VideoId = Split.Substring(0, 11);

            item.ExtraData = VideoId;
            item.UpdateState();
        }
    }
}
