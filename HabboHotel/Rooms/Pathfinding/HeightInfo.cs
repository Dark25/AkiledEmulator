namespace Akiled.HabboHotel.Rooms.Pathfinding
{
    public struct HeightInfo
    {
        private readonly double[,] _map;
        private readonly int _maxX;
        private readonly int _maxY;

        public HeightInfo(int MaxX, int MaxY, double[,] Map)
        {
            this._map = Map;
            this._maxX = MaxX;
            this._maxY = MaxY;
        }

        public double GetState(int x, int y)
        {
            if (x >= this._maxX || x < 0 || (y >= this._maxY || y < 0))
                return 0.0;
            else
                return this._map[x, y];
        }
    }
}
