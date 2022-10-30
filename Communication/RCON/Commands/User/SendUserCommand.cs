using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.RCON.Commands.User
{
    class SendUserCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            if (parameters.Length != 3)
                return false;
            int Userid = 0;
            if (!int.TryParse(parameters[1], out Userid))
                return false;
            if (Userid == 0)
                return false;

            GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);
            if (Client == null || Client.GetHabbo() == null)
                return false;

            int RoomId = 0;

            if (!int.TryParse(parameters[2], out RoomId))
                return false;
            if (RoomId == 0)
                return false;

            Room room = AkiledEnvironment.GetGame().GetRoomManager().LoadRoom(RoomId);
            if (room == null)
                return false;

            Client.SendPacket(new GetGuestRoomResultComposer(Client, room.RoomData, false, true));
            return true;
        }
    }
}
