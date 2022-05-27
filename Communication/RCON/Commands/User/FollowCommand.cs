using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.RCON.Commands.User
{
    class FollowCommand : IRCONCommand
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
            if (Client == null)
                return false;

            int Userid2 = 0;

            if (!int.TryParse(parameters[2], out Userid2))
                return false;
            if (Userid2 == 0)
                return false;

            GameClient Client2 = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid2);
            if (Client2 == null)
                return false;

            if (Client2.GetHabbo() == null || Client2.GetHabbo().CurrentRoom == null)
                return false;

            Room room = Client2.GetHabbo().CurrentRoom;
            if (room == null)
                return false;

            Client.SendPacket(new GetGuestRoomResultComposer(Client, room.RoomData, false, true));
            return true;
        }
    }
}
