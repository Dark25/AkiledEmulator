using Akiled.HabboHotel.Items;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class ObjectAddComposer : ServerPacket
    {
        string url_itemsexternal = (AkiledEnvironment.GetConfig().data["url_itemsexternal"]);
        public ObjectAddComposer(Item Item, string Username, int UserId)
            : base(ServerPacketHeader.ObjectAddMessageComposer)
        {
            WriteInteger(Item.Id);
            WriteInteger(Item.GetBaseItem().SpriteId);
            WriteInteger(Item.GetX);
            WriteInteger(Item.GetY);
            WriteInteger(Item.Rotation);
            WriteString(string.Format("{0:0.00}", Item.GetZ));
            WriteString(string.Empty);

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
            WriteInteger(1);
            WriteInteger(Item.OwnerId);
            WriteString(Username);
        }

        public ObjectAddComposer(ItemTemp Item)
            : base(ServerPacketHeader.ObjectAddMessageComposer)
        {
            WriteInteger(Item.Id);
            WriteInteger(Item.SpriteId); //ScriptId
            WriteInteger(Item.X);
            WriteInteger(Item.Y);
            WriteInteger(2);
            WriteString(string.Format("{0:0.00}", Item.Z));
            WriteString("");

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
                WriteString(Item.ExtraData); //ExtraData
            }


            WriteInteger(-1); // to-do: check
            WriteInteger(1);
            WriteInteger(Item.VirtualUserId);
            WriteString("");
        }
    }
}
