using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;using Akiled.HabboHotel.Rooms;using System;

namespace Akiled.Communication.Packets.Incoming.Structure{    class GetGuestRoomEvent : IPacketEvent    {        public void Parse(GameClient Session, ClientPacket Packet)        {
            int roomID = Packet.PopInt();

            RoomData roomData = AkiledEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomID);
            if (roomData == null)
                return;

            Boolean isLoading = Packet.PopInt() == 1;
            Boolean checkEntry = Packet.PopInt() == 1;

            Session.SendPacket(new GetGuestRoomResultComposer(Session, roomData, isLoading, checkEntry));        }    }}