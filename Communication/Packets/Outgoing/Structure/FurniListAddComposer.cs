﻿using Akiled.HabboHotel.Catalog.Utilities;
using Akiled.HabboHotel.Items;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class FurniListAddComposer : ServerPacket
    {
        public FurniListAddComposer(Item Item)
            : base(ServerPacketHeader.FurniListAddMessageComposer)
        {
            WriteInteger(Item.Id);
            WriteString(Item.GetBaseItem().Type.ToString().ToUpper());
            WriteInteger(Item.Id);
            WriteInteger(Item.GetBaseItem().SpriteId);

            if (Item.LimitedNo > 0)
            {
                WriteInteger(1);
                WriteInteger(256);
                WriteString(Item.ExtraData);
                WriteInteger(Item.LimitedNo);
                WriteInteger(Item.LimitedTot);
            }
            else
                ItemBehaviourUtility.GenerateExtradata(Item, this);

            WriteBoolean(Item.GetBaseItem().AllowEcotronRecycle);
            WriteBoolean(Item.GetBaseItem().AllowTrade);
            WriteBoolean(Item.LimitedNo == 0 && Item.GetBaseItem().AllowInventoryStack);
            WriteBoolean(ItemUtility.IsRare(Item));
            WriteInteger(-1);//Seconds to expiration.
            WriteBoolean(true);
            WriteInteger(-1);//Item RoomId

            if (!Item.IsWallItem)
            {
                WriteString(string.Empty);
                WriteInteger(0);
            }
        }
    }
}
