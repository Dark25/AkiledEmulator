using Akiled.HabboHotel.Rooms.Wired;
using System.Drawing;

namespace Akiled.HabboHotel.Rooms.Map.Movement
{
    public static class MovementManagement
    {
        public static void HandleMovement(ref Point coordinate, MovementState state)
        {
            switch (state)
            {
                case MovementState.up:
                    coordinate.Y--;
                    break;
                case MovementState.right:
                    coordinate.X++;
                    break;
                case MovementState.down:
                    coordinate.Y++;
                    break;
                case MovementState.left:
                    coordinate.X--;
                    break;
            }
        }

        public static Point HandleMovement(Point newCoordinate, MovementState state)
        {
            Point coordinate = new Point(newCoordinate.X, newCoordinate.Y);
            switch (state)
            {
                case MovementState.random:
                    switch (AkiledEnvironment.GetRandomNumber(1, 4))
                    {
                        case 1:
                            HandleMovement(ref coordinate, MovementState.up);
                            break;
                        case 2:
                            HandleMovement(ref coordinate, MovementState.down);
                            break;
                        case 3:
                            HandleMovement(ref coordinate, MovementState.left);
                            break;
                        case 4:
                            HandleMovement(ref coordinate, MovementState.right);
                            break;
                    }
                    break;
                case MovementState.leftright:
                    if (AkiledEnvironment.GetRandomNumber(0, 1) == 1)
                    {
                        HandleMovement(ref coordinate, MovementState.left);
                        break;
                    }
                    else
                    {
                        HandleMovement(ref coordinate, MovementState.right);
                        break;
                    }
                case MovementState.updown:
                    if (AkiledEnvironment.GetRandomNumber(0, 1) == 1)
                    {
                        HandleMovement(ref coordinate, MovementState.up);
                        break;
                    }
                    else
                    {
                        HandleMovement(ref coordinate, MovementState.down);
                        break;
                    }
                case MovementState.up:
                case MovementState.right:
                case MovementState.down:
                case MovementState.left:
                    HandleMovement(ref coordinate, state);
                    break;
            }
            return coordinate;
        }

        public static void HandleMovementDir(ref Point coordinate, MovementDirection state)
        {
            switch (state)
            {
                case MovementDirection.down:
                    {
                        coordinate.Y++;
                        break;
                    }

                case MovementDirection.up:
                    {
                        coordinate.Y--;
                        break;
                    }

                case MovementDirection.left:
                    {
                        coordinate.X--;
                        break;
                    }

                case MovementDirection.right:
                    {
                        coordinate.X++;
                        break;
                    }

                case MovementDirection.downright:
                    {
                        coordinate.X++;
                        coordinate.Y++;
                        break;
                    }

                case MovementDirection.downleft:
                    {
                        coordinate.X--;
                        coordinate.Y++;
                        break;
                    }

                case MovementDirection.upright:
                    {
                        coordinate.X++;
                        coordinate.Y--;
                        break;
                    }

                case MovementDirection.upleft:
                    {
                        coordinate.X--;
                        coordinate.Y--;
                        break;
                    }
            }
        }

        public static Point HandleMovementDir(int X, int Y, MovementDirection state)
        {
            Point newPoint = new Point(X, Y);

            switch (state)
            {
                case MovementDirection.up:
                case MovementDirection.down:
                case MovementDirection.left:
                case MovementDirection.right:
                case MovementDirection.downright:
                case MovementDirection.downleft:
                case MovementDirection.upright:
                case MovementDirection.upleft:
                    {
                        HandleMovementDir(ref newPoint, state);
                        break;
                    }

                case MovementDirection.random:
                    {
                        switch (AkiledEnvironment.GetRandomNumber(1, 4))
                        {
                            case 1:
                                {
                                    HandleMovementDir(ref newPoint, MovementDirection.up);
                                    break;
                                }
                            case 2:
                                {
                                    HandleMovementDir(ref newPoint, MovementDirection.down);
                                    break;
                                }

                            case 3:
                                {
                                    HandleMovementDir(ref newPoint, MovementDirection.left);
                                    break;
                                }
                            case 4:
                                {
                                    HandleMovementDir(ref newPoint, MovementDirection.right);
                                    break;
                                }
                        }
                        break;
                    }
            }

            return newPoint;
        }

        public static MovementDirection GetMovementByDirection(int Rot)
        {
            MovementDirection movement = MovementDirection.none;

            switch (Rot)
            {
                case 0:
                    movement = MovementDirection.up;
                    break;
                case 1:
                    movement = MovementDirection.upright;
                    break;
                case 2:
                    movement = MovementDirection.right;
                    break;
                case 3:
                    movement = MovementDirection.downright;
                    break;
                case 4:
                    movement = MovementDirection.down;
                    break;
                case 5:
                    movement = MovementDirection.downleft;
                    break;
                case 6:
                    movement = MovementDirection.left;
                    break;
                case 7:
                    movement = MovementDirection.upleft;
                    break;
            }

            return movement;
        }

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

        public static int HandleRotation(int oldRotation, RotationState state)
        {
            int rotation = oldRotation;
            switch (state)
            {
                case RotationState.clocwise:
                    HandleClockwiseRotation(ref rotation);
                    return rotation;
                case RotationState.counterClockwise:
                    HandleCounterClockwiseRotation(ref rotation);
                    return rotation;
                case RotationState.random:
                    if (AkiledEnvironment.GetRandomNumber(0, 1) == 1)
                        HandleClockwiseRotation(ref rotation);
                    else
                        HandleCounterClockwiseRotation(ref rotation);
                    return rotation;
                default:
                    return rotation;
            }
        }

        public static void HandleClockwiseRotation(ref int rotation)
        {
            rotation = rotation + 2;
            if (rotation <= 6)
                return;
            rotation = 0;
        }

        public static void HandleCounterClockwiseRotation(ref int rotation)
        {
            rotation = rotation - 2;
            if (rotation >= 0)
                return;
            rotation = 6;
        }
    }
}
