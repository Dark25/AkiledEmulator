namespace Akiled.HabboHotel.Rooms.Pathfinding
{
    public static class DreamPathfinder
    {
        public static SquarePoint GetNextStep(int pUserX, int pUserY, int pUserTargetX, int pUserTargetY, byte[,] pGameMap, double[,] pHeight, byte[,] pUserOnMap, byte[,] pSquareTaking, int MaxX, int MaxY, bool pUserOverride, bool pDiagonal, bool pAllowWalkthrough, bool pOblique)
        {
            ModelInfo pMap = new ModelInfo(MaxX, MaxY, pGameMap, pUserOnMap, pSquareTaking);
            SquarePoint pTarget = new SquarePoint(pUserTargetX, pUserTargetY, pUserTargetX, pUserTargetY, pMap.GetState(pUserTargetX, pUserTargetY), pUserOverride, pAllowWalkthrough, pMap.GetStateUser(pUserTargetX, pUserTargetY));
            if (pUserX == pUserTargetX && pUserY == pUserTargetY)
                return pTarget;
            else
                return GetClosetSqare(new SquareInformation(pUserX, pUserY, pTarget, pMap, pUserOverride, pDiagonal, pAllowWalkthrough, pOblique), new HeightInfo(MaxX, MaxY, pHeight), pUserOverride);
        }

        private static SquarePoint GetClosetSqare(SquareInformation pInfo, HeightInfo Height, bool pUserOverride)
        {
            double Closest = pInfo.Point.GetDistance;
            SquarePoint squarePoint1 = pInfo.Point;
            double state = Height.GetState(pInfo.Point.X, pInfo.Point.Y);

            for (int val = 0; val < 8; val++)
            {
                SquarePoint squarePoint2 = pInfo.Pos(val);
                if ((squarePoint2.AllowWalkthrough && squarePoint2.CanWalk && ((Height.GetState(squarePoint2.X, squarePoint2.Y) - state < 2)) || pUserOverride))
                {
                    double getDistance = squarePoint2.GetDistance;
                    if (Closest > getDistance)
                    {
                        Closest = getDistance;
                        squarePoint1 = squarePoint2;
                    }
                }
            }

            return squarePoint1;
        }

        public static double GetDistance(int x1, int y1, int x2, int y2)
        {
            int dx = (x1 - x2);
            int dy = (y1 - y2);
            return (dx * dx) + (dy * dy);
        }
    }
}
