using System;

namespace Astar.Algorithm
{
    public interface IPathNode
    {
        Boolean IsBlocked(int x, int y, bool lastTile);
    }
}
