using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.WebClients;

namespace Akiled.Communication.Packets.Incoming.WebSocket
{
    class RpBotChooseEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            string Message = Packet.PopString();

            GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetHabbo() == null)
                return;

            Room Room = Client.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Client.GetHabbo().Id);
            if (User == null)
                return;

            if (Room.AllowsShous(User, Message))
                User.SendWhisperChat(Message, false);
        }
    }
}