using Akiled.HabboHotel.Rooms.Map.Movement;
using System;

namespace Akiled.HabboHotel.Items
{
    public class ItemTemp : IEquatable<ItemTemp>
    {
        public int Id;
        public int VirtualUserId;
        public int SpriteId;
        public int X;
        public int Y;
        public double Z;
        public MovementDirection Movement;
        public string ExtraData;
        public int Value;
        public int TeamId;
        public int Distance;
        public InteractionTypeTemp InteractionType;

        public ItemTemp(int id, int userId, int spriteId, int x, int y, double z, string extraData, MovementDirection movement, int value, InteractionTypeTemp pInteraction, int pDistance = 0, int pTeamId = 0)
        {
            this.Id = id;
            this.VirtualUserId = userId;
            this.SpriteId = spriteId;
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.ExtraData = extraData;
            this.Movement = movement;
            this.Value = value;
            this.TeamId = pTeamId;
            this.Distance = pDistance;
            this.InteractionType = pInteraction;
        }

        public bool Equals(ItemTemp comparedItem)
        {
            return comparedItem.Id == this.Id;
        }
    }

    public enum InteractionTypeTemp
    {
        NONE,
        PROJECTILE,
        PROJECTILE_BOT,
        GRENADE,
        MONEY,
        RPITEM,
    }
}
