﻿using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Roleplay.Player;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.WebClients;

namespace Akiled.Communication.Packets.Incoming.WebSocket
{
    class RpTrocRemoveItemEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();

            GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetHabbo() == null)
                return;

            Room Room = Client.GetHabbo().CurrentRoom;
            if (Room == null || !Room.IsRoleplay)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Client.GetHabbo().Id);
            if (User == null)
                return;

            RolePlayer Rp = User.Roleplayer;
            if (Rp == null || Rp.TradeId == 0)
                return;

            AkiledEnvironment.GetGame().GetRoleplayManager().GetTrocManager().RemoveItem(Rp.TradeId, User.UserId, ItemId);
        }
    }
}
