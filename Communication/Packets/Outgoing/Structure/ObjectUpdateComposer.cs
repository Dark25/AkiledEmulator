using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired;
using System;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class ObjectUpdateComposer : ServerPacket
    {
        public ObjectUpdateComposer(Item Item, int UserId, bool HideWired = false)
            : base(ServerPacketHeader.ObjectUpdateMessageComposer)
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
