using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;
using System;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class ChangeFootGate : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int pId = Packet.PopInt();
            string gender = Packet.PopString();
            string look = Packet.PopString();

            Room room = Session.GetHabbo().CurrentRoom;
            if (room == null || !room.CheckRights(Session, true))
                return;

            Item item = room.GetRoomItemHandler().GetItem(pId);
            if (item == null || item.GetBaseItem().InteractionType != InteractionType.fbgate)
                return;

            if (gender.ToUpper() == "M")
            {
                string[] Figures = item.ExtraData.Split(',');
                string[] newFigures = new string[2];

                newFigures[0] = look;
                if (Figures.Length > 1)
                {
                    newFigures[1] = Figures[1];
                }
                else
                {
                    newFigures[1] = "hd-99999-99999.ch-630-62.lg-695-62";
                }

                item.ExtraData = String.Join(",", newFigures);
                //item.UpdateState();
                //    return;
            }
            else if (gender.ToUpper() == "F")
            {
                string[] Figures = item.ExtraData.Split(',');
                string[] newFigures = new string[2];

                if (!string.IsNullOrWhiteSpace(Figures[0]))
                {
                    newFigures[0] = Figures[0];
                }
                else
                {
                    newFigures[0] = "hd-99999-99999.lg-270-62";
                }
                newFigures[1] = look;

                item.ExtraData = String.Join(",", newFigures);
                //  item.UpdateState();
            }
        }
    }
}