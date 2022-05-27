namespace Akiled.HabboHotel.Rooms.Pathfinding
{
    public struct ModelInfo
    {
        private readonly byte[,] _map;
        private readonly int _maxX;
        private readonly int _maxY;
        private readonly byte[,] _userOnMap;
        private readonly byte[,] _squareTaking;

        public ModelInfo(int MaxX, int MaxY, byte[,] Map, byte[,] UserOnMap, byte[,] SquareTaking)
        {
            this._map = Map;
            this._maxX = MaxX;
            this._maxY = MaxY;
            this._userOnMap = UserOnMap;
            this._squareTaking = SquareTaking;
        }

        public byte GetStateUser(int x, int y)
        {
            if (x >= this._maxX || x < 0 || (y >= this._maxY || y < 0))
                return 1;

            if (this._userOnMap[x, y] == 1 || this._squareTaking[x, y] == 1)
                return 1;
            else
                return 0;
        }

        public byte GetState(int x, int y)
        {
            if (x >= this._maxX || x < 0 || (y >= this._maxY || y < 0))
                return (byte)0;
            else
                return this._map[x, y];
        }
    }
}