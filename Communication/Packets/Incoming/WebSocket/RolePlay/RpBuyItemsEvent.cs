using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Roleplay;
using Akiled.HabboHotel.Roleplay.Player;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.WebClients;

namespace Akiled.Communication.Packets.Incoming.WebSocket
{
    class RpBuyItemsEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            int Count = Packet.PopInt();

            if (Count > 99)
                Count = 99;
            if (Count < 1)
                Count = 1;

            GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetHabbo() == null)
                return;

            Room Room = Client.GetHabbo().CurrentRoom;
            if (Room == null || !Room.IsRoleplay)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Client.GetHabbo().Id);
            if (User == null)
                return;

            if (!User.AllowBuyItems.Contains(ItemId))
                return;

            RolePlayer Rp = User.Roleplayer;
            if (Rp == null || Rp.Dead || Rp.SendPrison)
                return;

            RPItem RpItem = AkiledEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(ItemId);
            if (RpItem == null)
                return;

            if (!RpItem.AllowStack && Rp.GetInventoryItem(RpItem.Id) != null)
            {
                User.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.itemown", Client.Langue));
                return;
            }

            if (!RpItem.AllowStack && Count > 1)
                Count = 1;

            if (Rp.Money < (RpItem.Price * Count))
                return;

            Rp.AddInventoryItem(RpItem.Id, Count);

            if (RpItem.Price == 0)
                User.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.itempick", Client.Langue));
            else
                User.SendWhisperChat(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.itembuy", Client.Langue), RpItem.Price));

            Rp.Money -= RpItem.Price * Count;
            Rp.SendUpdate();
        }
    }
}
