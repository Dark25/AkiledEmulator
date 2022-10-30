using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.Navigators;

namespace Akiled.HabboHotel.Rooms
{
    static class RoomAppender
    {
        public static void WriteRoom(ServerPacket Packet, RoomData Data)
        {
            Packet.WriteInteger(Data.Id);
            Packet.WriteString(Data.Name);
            Packet.WriteInteger(Data.OwnerId);
            Packet.WriteString(Data.OwnerName);
            Packet.WriteInteger(Data.State);
            Packet.WriteInteger(Data.UsersNow);
            Packet.WriteInteger((int)(Data.UsersMax / 1.5));
            Packet.WriteString(Data.Description);
            Packet.WriteInteger(Data.TrocStatus);
            Packet.WriteInteger(Data.Score);
            Packet.WriteInteger(1);//Top rated room rank.
            Packet.WriteInteger(Data.Category);

            Packet.WriteInteger(Data.Tags.Count);
            foreach (string tag in Data.Tags)
            {
                Packet.WriteString(tag);
            }

            int RoomType = 8;
            if (Data.Group != null)
                RoomType += 2;

            FeaturedRoom Item = null;
            if (AkiledEnvironment.GetGame().GetNavigator().TryGetFeaturedRoom(Data.Id, out Item))
            {
                RoomType += 1;
            }

            Packet.WriteInteger(RoomType);

            if (Item != null)
            {
                Packet.WriteString(Item.Image);
            }

            if (Data.Group != null)
            {
                Packet.WriteInteger(Data.Group == null ? 0 : Data.Group.Id);
                Packet.WriteString(Data.Group == null ? "" : Data.Group.Name);
                Packet.WriteString(Data.Group == null ? "" : Data.Group.Badge);
            }
        }
    }
}