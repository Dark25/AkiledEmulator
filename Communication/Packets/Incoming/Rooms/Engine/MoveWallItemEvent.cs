using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;
using System;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class MoveWallItemEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || !room.CheckRights(Session))
                return;
            int pId = Packet.PopInt();
            string str = Packet.PopString();
            Item roomItem = room.GetRoomItemHandler().GetItem(pId);
            if (roomItem == null)
                return;

            if (room.RoomData.SellPrice > 0)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("roomsell.error.7", Session.Langue));
                return;
            }

            string wallCoordinate = WallPositionCheck(":" + str.Split(':')[1]);
            roomItem.wallCoord = wallCoordinate;
            room.GetRoomItemHandler().UpdateItem(roomItem);

            room.SendPacket(new ItemUpdateComposer(roomItem, roomItem.OwnerId));

        }

        private string WallPositionCheck(string wallPosition)
        {
            //:w=3,2 l=9,63 l
            try
            {
                if (wallPosition.Contains(Convert.ToChar(13)))
                { return ":w=0,0 l=0,0 l"; }
                if (wallPosition.Contains(Convert.ToChar(9)))
                { return ":w=0,0 l=0,0 l"; }

                string[] posD = wallPosition.Split(' ');
                if (posD[2] != "l" && posD[2] != "r")
                    return ":w=0,0 l=0,0 l";

                string[] widD = posD[0].Substring(3).Split(',');
                int widthX = int.Parse(widD[0]);
                int widthY = int.Parse(widD[1]);
                //if (widthX < 0 || widthY < 0 || widthX > 200 || widthY > 200)
                //return ":w=0,0 l=0,0 l";

                string[] lenD = posD[1].Substring(2).Split(',');
                int lengthX = int.Parse(lenD[0]);
                int lengthY = int.Parse(lenD[1]);
                //if (lengthX < 0 || lengthY < 0 || lengthX > 200 || lengthY > 200)
                //return ":w=0,0 l=0,0 l";
                return ":w=" + widthX + "," + widthY + " " + "l=" + lengthX + "," + lengthY + " " + posD[2];
            }
            catch
            {
                return ":w=0,0 l=0,0 l";
            }
        }

    }
}
