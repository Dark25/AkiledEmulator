namespace Astar.Algorithm
{
    public interface IPathNodeContianer
    {
        IPathNode getPathNode(int y, int x);
        bool isBlocked(int y, int x);
        int GetLength(int dimenstion);

    }
}
