using System.Drawing;

namespace Akiled.HabboHotel.Rooms.Map.Movement
{
    static class Mouvement
    {
        public static Point GetMoveCoord(int X, int Y, int i, MovementDirection movementDir)
        {
            switch (movementDir)
            {
                case MovementDirection.up:
                    {
                        Y = Y - i;
                        break;
                    }
                case MovementDirection.upright:
                    {
                        X = X + i;
                        Y = Y - i;
                        break;
                    }
                case MovementDirection.right:
                    {
                        X = X + i;
                        break;
                    }
                case MovementDirection.downright:
                    {
                        X = X + i;
                        Y = Y + i;
                        break;
                    }
                case MovementDirection.down:
                    {
                        Y = Y + i;
                        break;
                    }
                case MovementDirection.downleft:
                    {
                        X = X - i;
                        Y = Y + i;
                        break;
                    }
                case MovementDirection.left:
                    {
                        X = X - i;
                        break;
                    }
                case MovementDirection.upleft:
                    {
                        X = X - i;
                        Y = Y - i;
                        break;
                    }
            }

            return new Point(X, Y);
        }
    }
}
