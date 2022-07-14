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
            this.WriteString(Item.Id.ToString());
            this.WriteInteger(Item.Data.SpriteId);
            try
            {
                this.WriteString(Item.wallCoord);
            }
            catch
            {
                this.WriteString("");
            }
            ItemBehaviourUtility.GenerateWallExtradata(Item, (ServerPacket)this);
            this.WriteInteger(-1);
            this.WriteInteger(Item.Data.Modes > 1 ? 1 : 0);
            this.WriteInteger(UserId);
        }
    }
}
