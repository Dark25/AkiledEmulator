using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class SetMannequinNameEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            string Name = Packet.PopString();

            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || !room.CheckRights(Session, true))
                return;

            Item roomItem = room.GetRoomItemHandler().GetItem(ItemId);
            if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.MANNEQUIN)
                return;
            string str2 = "";
            foreach (string str3 in Session.GetHabbo().Look.Split('.'))
            {
                if (str3.StartsWith("ch") || str3.StartsWith("lg") || str3.StartsWith("cc") || str3.StartsWith("ca") || str3.StartsWith("sh") || str3.StartsWith("wa"))
                    str2 = str2 + str3 + ".";
            }
            string habie = str2.Substring(0, str2.Length - 1);
            if (habie.Length > 200)
                habie = habie.Substring(0, 200);
            if (Name.Length > 100)
                Name = Name.Substring(0, 100);

            Name = Name.Replace(";", ":");

            roomItem.ExtraData = Session.GetHabbo().Gender.ToUpper() + ";" + habie + ";" + Name + ";";
            roomItem.UpdateState();
        }
    }
}