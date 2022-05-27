using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class TradingConfirmEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;
            Trade userTrade = room.GetUserTrade(Session.GetHabbo().Id);
            if (userTrade == null)
                return;
            userTrade.CompleteTrade(Session.GetHabbo().Id);
        }
    }
}