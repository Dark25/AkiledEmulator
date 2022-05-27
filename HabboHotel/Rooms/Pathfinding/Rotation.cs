namespace Akiled.HabboHotel.Rooms.Pathfinding
{
    public static class Rotation
    {
        public static int Calculate(int X1, int Y1, int X2, int Y2)
        {
            int num = 0;
            if (X1 > X2 && Y1 > Y2)
                num = 7;
            else if (X1 < X2 && Y1 < Y2)
                num = 3;
            else if (X1 > X2 && Y1 < Y2)
                num = 5;
            else if (X1 < X2 && Y1 > Y2)
                num = 1;
            else if (X1 > X2)
                num = 6;
            else if (X1 < X2)
                num = 2;
            else if (Y1 < Y2)
                num = 4;
            else if (Y1 > Y2)
                num = 0;
            return num;
        }

        public static int Calculate(int X1, int Y1, int X2, int Y2, bool moonwalk)
        {
            int rot = Calculate(X1, Y1, X2, Y2);
            if (!moonwalk)
                return rot;
            else
                return RotationIverse(rot);
        }

        public static int RotationIverse(int rot)
        {
            if (rot > 3)
                rot -= 4;
            else
                rot += 4;
            return rot;
        }
    }
}
