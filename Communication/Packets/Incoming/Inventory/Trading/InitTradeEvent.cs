using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Roleplay.Player;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class InitTradeEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;

            if(room.IsRoleplay)
            {
                RoomUser RoomUser = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                RoomUser RoomUserTarget = room.GetRoomUserManager().GetRoomUserByVirtualId(Packet.PopInt());
                if (RoomUser == null || RoomUser.GetClient() == null || RoomUser.GetClient().GetHabbo() == null)
                    return;
                if (RoomUserTarget == null || RoomUserTarget.GetClient() == null || RoomUserTarget.GetClient().GetHabbo() == null)
                    return;

                RolePlayer Rp = RoomUser.Roleplayer;
                if (Rp == null || Rp.TradeId > 0 || Rp.Dead || Rp.SendPrison || (Rp.PvpEnable && room.Pvp) || Rp.AggroTimer > 0)
                {
                    RoomUser.SendWhisperChat("Debes estar en zona segura para poder tradear y no en la guerra.");
                    return;
                }

                RolePlayer RpTarget = RoomUserTarget.Roleplayer;
                if (RpTarget == null || RpTarget.TradeId > 0 || RpTarget.Dead || RpTarget.SendPrison || (RpTarget.PvpEnable && room.Pvp) || RpTarget.AggroTimer > 0)
                {
                    RoomUser.SendWhisperChat("Este usuario no puede tradear.");
                    return;
                }

                AkiledEnvironment.GetGame().GetRoleplayManager().GetTrocManager().AddTrade(room.RoomData.OwnerId, RoomUser.UserId, RoomUserTarget.UserId, RoomUser.GetUsername(), RoomUserTarget.GetUsername());
                return;
            }

            if (room.RoomData.TrocStatus == 0)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.trade.error.1", Session.Langue));
                return;
            }
            else if (room.RoomData.TrocStatus == 1 && !room.CheckRights(Session))
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.trade.error.2", Session.Langue));
                return;
            }

            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            RoomUser roomUserByVirtualId = room.GetRoomUserManager().GetRoomUserByVirtualId(Packet.PopInt());
            if (roomUserByVirtualId == null || roomUserByVirtualId.GetClient() == null || roomUserByVirtualId.GetClient().GetHabbo() == null)
                return;

            if (!roomUserByVirtualId.GetClient().GetHabbo().AcceptTrading && Session.GetHabbo().Rank < 3)
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("user.tradedisabled", Session.Langue));
            else if (roomUserByVirtualId.transformation || roomUserByHabbo.transformation || roomUserByHabbo.IsSpectator || roomUserByVirtualId.IsSpectator)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.trade.error.3", Session.Langue));
            }
            else
                room.TryStartTrade(roomUserByHabbo, roomUserByVirtualId);
        }
    }
}