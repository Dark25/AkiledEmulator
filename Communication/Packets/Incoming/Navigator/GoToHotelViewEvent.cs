using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using System;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GoToHotelViewEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new CloseConnectionComposer());
            Session.GetHabbo().LoadingRoomId = 0;

            if (Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room != null)
            {
                room.GetRoomUserManager().RemoveUserFromRoom(Session, false, false);
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("The user: " + Session.GetHabbo().Username + " has left the hotel view.");
            }
        }
    }
}