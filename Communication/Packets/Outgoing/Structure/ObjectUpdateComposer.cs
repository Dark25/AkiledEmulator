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

        public class UpdateFootBallComposer : ServerPacket
        {

            public UpdateFootBallComposer(Item Item, int newX, int newY)
                : base(ServerPacketHeader.ObjectUpdateMessageComposer)
            {
                WriteInteger(Item.Id);
                WriteInteger(Item.GetBaseItem().SpriteId);
                WriteInteger(newX);
                WriteInteger(newY);
                WriteInteger(4); // rot;
                WriteString((String.Format("{0:0.00}", Item.GetZ)));
                WriteString((String.Format("{0:0.00}", Item.GetZ)));
                WriteInteger(0);
                WriteInteger(0);
                WriteString(Item.ExtraData);
                WriteInteger(-1);
                WriteInteger(0);
                WriteInteger(Item.OwnerId);
            }
        }
    }
}
