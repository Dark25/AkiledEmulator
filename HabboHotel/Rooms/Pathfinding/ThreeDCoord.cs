using System;
using System.Drawing;

namespace Akiled.HabboHotel.Rooms.Pathfinding
{
    public struct ThreeDCoord : IEquatable<ThreeDCoord>
    {
        public int X;
        public int Y;
        public int Z;

        public ThreeDCoord(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static bool operator ==(ThreeDCoord a, ThreeDCoord b)
        {
            if (a.X == b.X && a.Y == b.Y)
                return a.Z == b.Z;
            else
                return false;
        }

        public static bool operator !=(ThreeDCoord a, ThreeDCoord b)
        {
            return !(a == b);
        }

        public bool Equals(ThreeDCoord comparedCoord)
        {
            if (this.X == comparedCoord.X && this.Y == comparedCoord.Y)
                return this.Z == comparedCoord.Z;
            else
                return false;
        }

        public bool Equals(Point comparedCoord)
        {
            if (this.X == comparedCoord.X)
                return this.Y == comparedCoord.Y;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.X ^ this.Y ^ this.Z;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            else
                return base.GetHashCode().Equals(obj.GetHashCode());
        }
    }
}
