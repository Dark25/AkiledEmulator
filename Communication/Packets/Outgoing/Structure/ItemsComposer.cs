using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class ItemsComposer : ServerPacket
    {
        public ItemsComposer(Item[] Objects, Room Room)
            : base(ServerPacketHeader.ItemsMessageComposer)
        {
            WriteInteger(1);
            WriteInteger(Room.RoomData.OwnerId);
            WriteString(Room.RoomData.OwnerName);

            WriteInteger(Objects.Length);

            foreach (Item Item in Objects)
            {
                WriteWallItem(Item, Item.OwnerId);
            }
        }

        private void WriteWallItem(Item Item, int UserId)
        {
            WriteString(Item.Id.ToString());
            WriteInteger(Item.GetBaseItem().SpriteId);

            try
            {
                WriteString(Item.wallCoord);
            }
            catch
            {
                WriteString("");
            }

            ItemBehaviourUtility.GenerateWallExtradata(Item, this);

            WriteInteger(-1);
            WriteInteger((Item.GetBaseItem().Modes > 1) ? 1 : 0);
            WriteInteger(UserId);
        }
    }
}
