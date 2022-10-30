using AStar.Algorithm;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Astar.Algorithm
{


    /// <summary>
    /// Uses about 50 MB for a 1024x1024 grid.
    /// </summary>
    public class AStarSolver<TPathNode> where TPathNode : IPathNode
    {
        #region declares
        private delegate Double CalculateHeuristicDelegate(PathNode inStart, PathNode inEnd);
        private CalculateHeuristicDelegate CalculationMethod;
        private static readonly Double SQRT_2 = Math.Sqrt(2);
        public Double tieBreaker;
        private readonly bool AllowDiagonal;
        private PathNode startNode;
        private PathNode endNode;
        private bool[,] mClosedSet;
        private bool[,] mOpenSet;
        private PriorityQueue<PathNode, Double> mOrderedOpenSet;
        private PathNode[,] mSearchSpace;
        private int nodes;
        private int Size;

        public TPathNode SearchSpace { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        #endregion

        #region constructor

        /// <summary>
        /// Creates a new AstarSolver
        /// </summary>
        /// <param name="inGrid">The inut grid</param>
        /// <param name="allowDiagonal">Indication if diagonal is allowed</param>
        /// <param name="calculator">The Calculator method</param>
        public AStarSolver(bool allowDiagonal, AStarHeuristicType calculator, TPathNode inGrid, int width, int height)
        {
            setHeuristictype(calculator);
            this.AllowDiagonal = allowDiagonal;
            prepareMap(inGrid, width, height);
        }
        #endregion
        private void prepareMap(TPathNode inGrid, int width, int height)
        {
            SearchSpace = inGrid;
            Width = width;//inGrid.GetLength(1);
            Height = height;//inGrid.GetLength(0);
            Size = Width * Height;
            mSearchSpace = new PathNode[Height, Width];
            mOrderedOpenSet = new PriorityQueue<PathNode, double>(PathNode.Comparer, Width + Height);

        }

        private void resetSearchSpace()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    mSearchSpace[y, x] = new PathNode(x, y, SearchSpace);
                }
            }
        }

        #region calculation types setting
        /// <summary>
        /// Sets the calculation type
        /// </summary>
        /// <param name="calculator"></param>
        private void setHeuristictype(AStarHeuristicType calculator)
        {
            switch (calculator)
            {
                case AStarHeuristicType.FAST_SEARCH: this.CalculationMethod = CalculateHeuristicFast; break;
                case AStarHeuristicType.BETWEEN: this.CalculationMethod = CalculateHeuristicBetween; break;
                case AStarHeuristicType.SHORTEST_PATH: this.CalculationMethod = CalculateHeuristicShortestRoute; break;
                case AStarHeuristicType.EXPERIMENTAL_SEARCH: this.CalculationMethod = CalculateHeuristicExperimental; break;
            }
        }

        protected virtual Double CalculateHeuristicExperimental(PathNode inStart, PathNode inEnd)
        {
            return CalculateHeuristicFast(inStart, inEnd);

        }

        protected virtual Double CalculateHeuristicFast(PathNode inStart, PathNode inEnd)
        {

            Double dx1 = inStart.X - endNode.X;
            Double dy1 = inStart.Y - endNode.Y;
            Double cross = Math.Abs(dx1 - dy1);
            return Math.Ceiling((Double)Math.Abs(inStart.X - inEnd.X) + (Double)Math.Abs(inStart.Y - inEnd.Y)) + cross;

        }

        protected virtual Double CalculateHeuristicBetween(PathNode inStart, PathNode inEnd)
        {
            Double dx1 = inStart.X - endNode.X;
            Double dy1 = inStart.Y - endNode.Y;
            Double dx2 = startNode.X - endNode.X;
            Double dy2 = startNode.Y - endNode.Y;
            Double cross = Math.Abs(dx1 * dy2 - dx2 * dy1);
            return Math.Ceiling((Double)Math.Abs(inStart.X - inEnd.X) + (Double)Math.Abs(inStart.Y - inEnd.Y)) + cross;
        }

        protected virtual Double CalculateHeuristicShortestRoute(PathNode inStart, PathNode inEnd)
        {
            return Math.Sqrt((inStart.X - inEnd.X) * (inStart.X - inEnd.X) + (inStart.Y - inEnd.Y) * (inStart.Y - inEnd.Y));
        }

        #endregion

        #region neighbour calculation
        /// <summary>
        /// Calculates the neighbour distance
        /// </summary>
        /// <param name="inStart">Start node</param>
        /// <param name="inEnd">End node</param>
        /// <returns></returns>
        protected virtual Double NeighborDistance(PathNode inStart, PathNode inEnd)
        {
            int diffX = Math.Abs(inStart.X - inEnd.X);
            int diffY = Math.Abs(inStart.Y - inEnd.Y);

            switch (diffX + diffY)
            {
                case 1: return 1;
                case 2: return SQRT_2;
                default:
                    throw new ApplicationException();
            }
        }
        #endregion

        #region search algo
        /// <summary>
        /// Returns null, if no path is found. Start- and End-Node are included in returned path. The user context
        /// is passed to IsWalkable().
        /// </summary>
        public LinkedList<PathNode> Search(Point inEndNode, Point inStartNode) //TPathNode inGrid, int width, int height)
        {
            //prepareMap(inGrid, width, height);
            //if (width < inStartNode.X || height < inStartNode.Y)
            //    return null;
            //if (width < inEndNode.X || height < inEndNode.Y)
            //    return null;
            resetSearchSpace();
            mOrderedOpenSet = new PriorityQueue<PathNode, double>(PathNode.Comparer, Width + Height);

            mClosedSet = new bool[Height, Width];
            mOpenSet = new bool[Height, Width];

            startNode = mSearchSpace[inStartNode.Y, inStartNode.X];
            endNode = mSearchSpace[inEndNode.Y, inEndNode.X];



            if (startNode == endNode)
                return new LinkedList<PathNode>(new PathNode[] { startNode });
            PathNode[] neighborNodes;
            if (AllowDiagonal)
                neighborNodes = new PathNode[8];
            else
                neighborNodes = new PathNode[4];



            tieBreaker = 0;

            startNode.G = 0;
            startNode.Optimal = CalculationMethod(startNode, endNode);
            tieBreaker = 1d / startNode.Optimal;
            startNode.F = startNode.Optimal;

            mOrderedOpenSet.Push(startNode);
            startNode.extraWeight = Size;
            nodes = 0;

            Double trailScore;
            Boolean wasAdded;
            Boolean scoreResultBetter;
            PathNode y;
            PathNode x;

            while ((x = mOrderedOpenSet.Pop()) != null)
            {
                if (x == endNode)
                {
                    LinkedList<PathNode> result = ReconstructPath(x);

                    result.AddLast(endNode);

                    return result;
                }

                mClosedSet[x.Y, x.X] = true;

                if (AllowDiagonal)
                    StoreNeighborNodesDiagonal(x, neighborNodes);
                else
                    StoreNeighborNodesNoDiagonal(x, neighborNodes);

                for (int i = 0; i < neighborNodes.Length; i++)
                {
                    y = neighborNodes[i];

                    if (y == null)
                        continue;

                    if (y.UserItem.IsBlocked(y.X, y.Y, (endNode.X == y.X && endNode.Y == y.Y)))
                        continue;

                    if (mClosedSet[y.Y, y.X])
                        continue;

                    nodes++;

                    trailScore = y.G + 1;
                    wasAdded = false;

                    if (mOpenSet[y.Y, y.X] == false)
                    {
                        mOpenSet[y.Y, y.X] = true;
                        scoreResultBetter = true;
                        wasAdded = true;
                    }
                    else if (trailScore < y.G)
                    {
                        scoreResultBetter = true;
                    }
                    else
                    {
                        scoreResultBetter = false;
                    }

                    if (scoreResultBetter)
                    {
                        y.parent = x;

                        if (wasAdded)
                        {
                            y.G = trailScore;
                            y.Optimal = CalculateHeuristicBetween(y, endNode) + (x.extraWeight - 10);
                            y.extraWeight = x.extraWeight - 10;
                            y.F = y.G + y.Optimal;
                            mOrderedOpenSet.Push(y);
                        }

                        else
                        {
                            y.G = trailScore;
                            y.Optimal = CalculateHeuristicBetween(y, endNode) + (x.extraWeight - 10);
                            mOrderedOpenSet.Update(y, y.G + y.Optimal);
                            y.extraWeight = x.extraWeight - 10;
                        }
                    }
                }
            }

            return null;
        }
        #endregion

        #region neighbour storing
        private void StoreNeighborNodesDiagonal(PathNode inAround, PathNode[] inNeighbors)
        {
            int x = inAround.X;
            int y = inAround.Y;

            if ((x > 0) && (y > 0))
            {
                inNeighbors[0] = mSearchSpace[y - 1, x - 1];
            }
            else
                inNeighbors[0] = null;

            if (y > 0)
                inNeighbors[1] = mSearchSpace[y - 1, x];
            else
                inNeighbors[1] = null;

            if ((x < Width - 1) && (y > 0))
            {
                inNeighbors[2] = mSearchSpace[y - 1, x + 1];
            }
            else
                inNeighbors[2] = null;

            if (x > 0)
                inNeighbors[3] = mSearchSpace[y, x - 1];
            else
                inNeighbors[3] = null;

            if (x < Width - 1)
                inNeighbors[4] = mSearchSpace[y, x + 1];
            else
                inNeighbors[4] = null;

            if ((x > 0) && (y < Height - 1))
            {
                inNeighbors[5] = mSearchSpace[y + 1, x - 1];

            }
            else
                inNeighbors[5] = null;

            if (y < Height - 1)
                inNeighbors[6] = mSearchSpace[y + 1, x];
            else
                inNeighbors[6] = null;

            if ((x < Width - 1) && (y < Height - 1))
            {
                inNeighbors[7] = mSearchSpace[y + 1, x + 1];
            }
            else
                inNeighbors[7] = null;
        }
        private void StoreNeighborNodesNoDiagonal(PathNode inAround, PathNode[] inNeighbors)
        {
            int x = inAround.X;
            int y = inAround.Y;

            if (y > 0)
                inNeighbors[0] = mSearchSpace[y - 1, x];
            else
                inNeighbors[0] = null;

            if (x > 0)
                inNeighbors[1] = mSearchSpace[y, x - 1];
            else
                inNeighbors[1] = null;

            if (x < Width - 1)
                inNeighbors[2] = mSearchSpace[y, x + 1];
            else
                inNeighbors[2] = null;

            if (y < Height - 1)
                inNeighbors[3] = mSearchSpace[y + 1, x];
            else
                inNeighbors[3] = null;
        }
        #endregion

        #region reconstructPath
        private LinkedList<PathNode> ReconstructPath(PathNode current_node)
        {
            LinkedList<PathNode> result = new LinkedList<PathNode>();

            ReconstructPathRecursive(current_node, result);

            return result;
        }
        private void ReconstructPathRecursive(PathNode current_node, LinkedList<PathNode> result)
        {
            PathNode item = current_node;
            result.AddFirst(item);
            while ((item = item.parent) != null)
            {
                result.AddFirst(item);
                current_node = item;
            }
        }

        #endregion

        #region openmap
        //private class OpenCloseMap
        //{
        //    private PathNode[,] m_Map;
        //    public int Width { get; private set; }
        //    public int Height { get; private set; }
        //    public int Count { get; private set; }

        //    public PathNode this[Int32 x, Int32 y]
        //    {
        //        get
        //        {
        //            return m_Map[x, y];
        //        }
        //    }

        //    public PathNode this[PathNode Node]
        //    {
        //        get
        //        {
        //            return m_Map[Node.X, Node.Y];
        //        }

        //    }

        //    public bool IsEmpty
        //    {
        //        get
        //        {
        //            return Count == 0;
        //        }
        //    }

        //    public OpenCloseMap(int inWidth, int inHeight)
        //    {
        //        m_Map = new PathNode[inWidth, inHeight];
        //        Width = inWidth;
        //        Height = inHeight;
        //    }

        //    public void Add(PathNode inValue)
        //    {
        //        PathNode item = m_Map[inValue.X, inValue.Y];
        //        Count++;
        //        m_Map[inValue.X, inValue.Y] = inValue;
        //    }

        //    public bool Contains(PathNode inValue)
        //    {
        //        PathNode item = m_Map[inValue.X, inValue.Y];

        //        if (item == null)
        //            return false;
        //        return true;
        //    }

        //    public void Remove(PathNode inValue)
        //    {
        //        PathNode item = m_Map[inValue.X, inValue.Y];
        //        Count--;
        //        m_Map[inValue.X, inValue.Y] = null;
        //    }
        //}
        #endregion

        #region path node class
        public class PathNode : IPathNode, IComparer<PathNode>, IWeightAddable<double>
        {
            public static readonly PathNode Comparer = new PathNode(0, 0, default(TPathNode));

            public TPathNode UserItem { get; internal set; }
            public Double G { get; internal set; }
            public Double Optimal { get; internal set; }
            public Double F { get; internal set; }

            public PathNode parent;

            public Boolean IsBlocked(int X, int Y, bool lastTile)
            {
                return UserItem.IsBlocked(X, Y, lastTile);
            }


            public int X { get; internal set; }
            public int Y { get; internal set; }
            public int extraWeight;
            public int Compare(PathNode x, PathNode y)
            {
                if (x.F < y.F)
                    return -1;
                else if (x.F > y.F)
                    return 1;

                return 0;
            }

            public PathNode(int inX, int inY, TPathNode inUserContext)
            {
                X = inX;
                Y = inY;
                UserItem = inUserContext;
            }


            public double WeightChange
            {
                get
                {
                    return F;
                }
                set
                {
                    F = value;
                }
            }

            public bool BeenThere;
        }
        #endregion

    }
}
