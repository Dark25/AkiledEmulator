using Astar.Algorithm;
using Enclosure.Algorithm;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Enclosure
{
    public class GameField : IPathNode
    {
        private readonly Queue<GametileUpdate> newEntries = new Queue<GametileUpdate>();
        private byte[,] currentField;
        private readonly AStarSolver<GameField> astarSolver;
        private readonly bool diagonal;
        private GametileUpdate currentlyChecking;

        public bool this[int y, int x]
        {
            get
            {
                return y >= 0 && x >= 0 && (y <= this.currentField.GetUpperBound(0) && x <= this.currentField.GetUpperBound(1));
            }
        }

        public GameField(byte[,] theArray, bool diagonalAllowed)
        {
            this.currentField = theArray;
            this.diagonal = diagonalAllowed;
            this.astarSolver = new AStarSolver<GameField>(diagonalAllowed, AStarHeuristicType.EXPERIMENTAL_SEARCH, this, theArray.GetUpperBound(1) + 1, theArray.GetUpperBound(0) + 1);
        }

        public void updateLocation(int x, int y, byte value)
        {
            this.newEntries.Enqueue(new GametileUpdate(x, y, value));
        }

        public List<PointField> doUpdate(bool oneloop = false)
        {
            List<PointField> list = new List<PointField>();
            while (this.newEntries.Count > 0)
            {
                this.currentlyChecking = this.newEntries.Dequeue();

                this.currentField[this.currentlyChecking.y, this.currentlyChecking.x] = this.currentlyChecking.value;

                List<Point> connectedItems = this.getConnectedItems(this.currentlyChecking);
                if (connectedItems.Count > 1)
                {
                    foreach (LinkedList<AStarSolver<GameField>.PathNode> nodeList in this.handleListOfConnectedPoints(connectedItems, this.currentlyChecking))
                    {
                        if (nodeList.Count >= 4)
                        {
                            PointField closed = this.findClosed(nodeList, this.currentlyChecking.value);
                            if (closed != null)
                                list.Add(closed);
                        }
                    }
                }
            }
            return list;
        }

        private PointField findClosed(LinkedList<AStarSolver<GameField>.PathNode> nodeList, byte Team)
        {
            PointField pointField = new PointField(this.currentlyChecking.value);
            int num1 = int.MaxValue;
            int num2 = int.MinValue;
            int num3 = int.MaxValue;
            int num4 = int.MinValue;
            foreach (AStarSolver<GameField>.PathNode pathNode in nodeList)
            {
                if (pathNode.X < num1)
                    num1 = pathNode.X;
                if (pathNode.X > num2)
                    num2 = pathNode.X;
                if (pathNode.Y < num3)
                    num3 = pathNode.Y;
                if (pathNode.Y > num4)
                    num4 = pathNode.Y;
            }
            int x1 = (int)Math.Ceiling((double)(num2 - num1) / 2.0) + num1;
            int y1 = (int)Math.Ceiling((double)(num4 - num3) / 2.0) + num3;
            List<Point> list1 = new List<Point>();
            List<Point> list2 = new List<Point>();
            list2.Add(new Point(this.currentlyChecking.x, this.currentlyChecking.y));
            list1.Add(new Point(x1, y1));
            while (list1.Count > 0)
            {
                Point p = list1[0];
                int x2 = p.X;
                int y2 = p.Y;
                if (x2 < num1)
                    return (PointField)null;
                if (x2 > num2)
                    return (PointField)null;
                if (y2 < num3)
                    return (PointField)null;
                if (y2 > num4)
                    return (PointField)null;
                Point point;
                if (this[y2 - 1, x2] && this.currentField[y2 - 1, x2] != Team)
                {
                    point = new Point(x2, y2 - 1);
                    if (!list1.Contains(point) && !list2.Contains(point))
                        list1.Add(point);
                }
                if (this[y2 + 1, x2] && this.currentField[y2 + 1, x2] != Team)
                {
                    point = new Point(x2, y2 + 1);
                    if (!list1.Contains(point) && !list2.Contains(point))
                        list1.Add(point);
                }
                if (this[y2, x2 - 1] && this.currentField[y2, x2 - 1] != Team)
                {
                    point = new Point(x2 - 1, y2);
                    if (!list1.Contains(point) && !list2.Contains(point))
                        list1.Add(point);
                }
                if (this[y2, x2 + 1] && this.currentField[y2, x2 + 1] != Team)
                {
                    point = new Point(x2 + 1, y2);
                    if (!list1.Contains(point) && !list2.Contains(point))
                        list1.Add(point);
                }
                if (this.getValue(p) != Team)
                    pointField.add(p);
                list2.Add(p);
                list1.RemoveAt(0);
            }
            return pointField;
        }

        private List<LinkedList<AStarSolver<GameField>.PathNode>> handleListOfConnectedPoints(List<Point> pointList, GametileUpdate update)
        {
            List<LinkedList<AStarSolver<GameField>.PathNode>> list = new List<LinkedList<AStarSolver<GameField>.PathNode>>();
            int num = 0;
            foreach (Point inStartNode in pointList)
            {
                ++num;
                if (num == pointList.Count / 2 + 1)
                    return list;
                foreach (Point inEndNode in pointList)
                {
                    if (!(inStartNode == inEndNode))
                    {
                        LinkedList<AStarSolver<GameField>.PathNode> linkedList = this.astarSolver.Search(inEndNode, inStartNode);
                        if (linkedList != null)
                            list.Add(linkedList);
                    }
                }
            }
            return list;
        }

        private List<Point> getConnectedItems(GametileUpdate update)
        {
            List<Point> list = new List<Point>();
            int x = update.x;
            int y = update.y;
            if (this.diagonal)
            {
                if (this[y - 1, x - 1] && this.currentField[y - 1, x - 1] == update.value)
                    list.Add(new Point(x - 1, y - 1));
                if (this[y - 1, x + 1] && this.currentField[y - 1, x + 1] == update.value)
                    list.Add(new Point(x + 1, y - 1));
                if (this[y + 1, x - 1] && this.currentField[y + 1, x - 1] == update.value)
                    list.Add(new Point(x - 1, y + 1));
                if (this[y + 1, x + 1] && this.currentField[y + 1, x + 1] == update.value)
                    list.Add(new Point(x + 1, y + 1));
            }
            if (this[y - 1, x] && this.currentField[y - 1, x] == update.value)
                list.Add(new Point(x, y - 1));
            if (this[y + 1, x] && this.currentField[y + 1, x] == update.value)
                list.Add(new Point(x, y + 1));
            if (this[y, x - 1] && this.currentField[y, x - 1] == update.value)
                list.Add(new Point(x - 1, y));
            if (this[y, x + 1] && this.currentField[y, x + 1] == update.value)
                list.Add(new Point(x + 1, y));
            return list;
        }

        private void setValue(int x, int y, byte value)
        {
            if (!this[y, x])
                return;
            this.currentField[y, x] = value;
        }

        public byte getValue(int x, int y)
        {
            if (this[y, x])
                return this.currentField[y, x];
            else
                return (byte)0;
        }

        public byte getValue(Point p)
        {
            if (this[p.Y, p.X])
                return this.currentField[p.Y, p.X];
            else
                return (byte)0;
        }

        public bool IsBlocked(int x, int y, bool lastTile)
        {
            if (this.currentlyChecking.x == x && this.currentlyChecking.y == y)
                return true;
            else
                return this.getValue(x, y) != this.currentlyChecking.value;
        }

        public void destroy()
        {
            this.currentField = (byte[,])null;
        }
    }
}
