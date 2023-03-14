using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Rooms.Wired;
using System;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class ObjectsComposer : ServerPacket
    {
        string url_itemsexternal = (AkiledEnvironment.GetConfig().data["url_itemsexternal"]);
        public ObjectsComposer(Item[] Objects, Room Room)
            : base(ServerPacketHeader.ObjectsMessageComposer)
        {
            WriteInteger(1);

            WriteInteger(Room.RoomData.OwnerId);
            WriteString(Room.RoomData.OwnerName);

            WriteInteger(Objects.Length);
            foreach (Item Item in Objects)
            {
                WriteFloorItem(Item, Item.OwnerId, Room.HideWired);
            }
        }

        public ObjectsComposer(ItemTemp[] Objects, Room Room)
            : base(ServerPacketHeader.ObjectsMessageComposer)
        {
            WriteInteger(1);

            WriteInteger(Room.RoomData.OwnerId);
            WriteString(Room.RoomData.OwnerName);

            WriteInteger(Objects.Length);
            foreach (ItemTemp Item in Objects)
            {
                WriteFloorItem(Item, Convert.ToInt32(Room.RoomData.OwnerId));
            }
        }

        private void WriteFloorItem(ItemTemp Item, int UserID)
        {

            WriteInteger(Item.Id);
            WriteInteger(Item.SpriteId);
            WriteInteger(Item.X);
            WriteInteger(Item.Y);
            WriteInteger(2);
            WriteString(String.Format("{0:0.00}", Item.Z));
            WriteString(String.Empty);

            if (Item.InteractionType == InteractionTypeTemp.RPITEM)
            {
                WriteInteger(0);
                WriteInteger(1);

                WriteInteger(5);

                WriteString("state");
                WriteString("0");
                WriteString("imageUrl");
                WriteString(url_itemsexternal + Item.ExtraData + ".png");
                WriteString("offsetX");
                WriteString("-20");
                WriteString("offsetY");
                WriteString("10");
                WriteString("offsetZ");
                WriteString("10002");
            }
            else
            {
                WriteInteger(1);
                WriteInteger(0);
                WriteString(Item.ExtraData);
            }

            WriteInteger(-1); // to-do: check
            WriteInteger(1); //(Item.GetBaseItem().Modes > 1) ? 1 : 0
            WriteInteger(UserID);
        }

        private void WriteFloorItem(Item Item, int UserID, bool HideWired)
        {

            WriteInteger(Item.Id);
            WriteInteger((HideWired && WiredUtillity.TypeIsWired(Item.GetBaseItem().InteractionType) && (Item.GetBaseItem().InteractionType != InteractionType.highscore && Item.GetBaseItem().InteractionType != InteractionType.highscorepoints)) ? 31294061 : Item.GetBaseItem().SpriteId);
            WriteInteger(Item.GetX);
            WriteInteger(Item.GetY);
            WriteInteger(Item.Rotation);
            WriteString(String.Format("{0:0.00}", Item.GetZ));
            WriteString(String.Empty);

            if (Item.LimitedNo > 0)
            {
                WriteInteger(1);
                WriteInteger(256);
                WriteString(Item.ExtraData);
                WriteInteger(Item.LimitedNo);
                WriteInteger(Item.LimitedTot);
            }
            else if (Item.Data.InteractionType == InteractionType.PLANT_SEED)
            {
                base.WriteInteger(0);
                base.WriteInteger(7);
                base.WriteString(Item.ExtraData);
                if (Item.ExtraData.Length <= 0)
                {
                    base.WriteInteger(0);
                }
                else
                {
                    base.WriteInteger(int.Parse(Item.ExtraData));
                }
                base.WriteInteger(12);
            }
            else
            {
                ItemBehaviourUtility.GenerateExtradata(Item, this);
            }

            WriteInteger(-1); // to-do: check
            WriteInteger(1); //(Item.GetBaseItem().Modes > 1) ? 1 : 0
            WriteInteger(Item.OwnerId);
        }
    }
}
