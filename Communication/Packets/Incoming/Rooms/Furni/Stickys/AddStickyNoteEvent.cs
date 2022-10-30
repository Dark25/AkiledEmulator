using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;
using System;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class AddStickyNoteEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || !room.CheckRights(Session))
                return;
            int Id = Packet.PopInt();
            string str = Packet.PopString();
            Item userItem = Session.GetHabbo().GetInventoryComponent().GetItem(Id);
            if (userItem == null)
                return;
            if (room == null)
                return;

            string wallCoord = WallPositionCheck(":" + str.Split(':')[1]);
            Item roomItem = new Item(userItem.Id, userItem.OwnerId, room.Id, userItem.BaseItem, userItem.ExtraData, userItem.LimitedNo, userItem.LimitedTot, 0, 0, 0.0, 0, wallCoord, room);
            if (!room.GetRoomItemHandler().SetWallItem(Session, roomItem))
                return;

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery("UPDATE items SET room_id = " + room.Id + ", user_id = " + roomItem.OwnerId + " WHERE id = " + Id);

            Session.GetHabbo().GetInventoryComponent().RemoveItem(Id);

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
