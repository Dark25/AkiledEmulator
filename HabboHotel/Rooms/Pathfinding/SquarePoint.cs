namespace Akiled.HabboHotel.Rooms.Pathfinding
{
    public readonly struct SquarePoint
    {
        private readonly int _x;
        private readonly int _y;
        private readonly double _distance;
        private readonly byte _squareData;
        private readonly bool _override;
        private readonly bool _lastStep;
        private readonly bool _allowWalkthrough;
        private readonly byte _squareDataUser;

        public SquarePoint(int pX, int pY, int pTargetX, int pTargetY, byte SquareData, bool pOverride, bool pAllowWalkthrough, byte SquareDataUser)
        {
            this._x = pX;
            this._y = pY;
            this._squareData = SquareData;
            this._squareDataUser = SquareDataUser;
            this._override = pOverride;
            this._distance = 0.0;
            this._lastStep = pX == pTargetX && pY == pTargetY;
            this._distance = DreamPathfinder.GetDistance(pX, pY, pTargetX, pTargetY);
            this._allowWalkthrough = pAllowWalkthrough;
        }

        public int X
        {
            get
            {
                return this._x;
            }
        }

        public int Y
        {
            get
            {
                return this._y;
            }
        }

        public double GetDistance
        {
            get
            {
                return this._distance;
            }
        }

        public bool CanWalk
        {
            get
            {
                if (this._lastStep)
                    return this._override || this._squareData == 3 || this._squareData == 1;
                else
                    return this._override || this._squareData == 1;
            }
        }

        public bool AllowWalkthrough
        {
            get
            {
                return this._allowWalkthrough || _squareDataUser == 0;
            }
        }
    }
}
