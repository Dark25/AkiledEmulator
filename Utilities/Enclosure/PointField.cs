using System.Collections.Generic;
using System.Drawing;

namespace Enclosure
{
    public class PointField
    {
        private static readonly Point badPoint = new Point(-1, -1);
        private Point mostLeft = badPoint;
        private Point mostTop = badPoint;
        private Point mostRight = badPoint;
        private Point mostDown = badPoint;
        private readonly List<Point> PointList;

        public byte forValue { get; private set; }

        static PointField()
        {
        }

        public PointField(byte forValue)
        {
            this.PointList = new List<Point>();
            this.forValue = forValue;
        }

        public List<Point> getPoints()
        {
            return this.PointList;
        }

        public void add(Point p)
        {
            if (this.mostLeft == badPoint)
                this.mostLeft = p;
            if (this.mostRight == badPoint)
                this.mostRight = p;
            if (this.mostTop == badPoint)
                this.mostTop = p;
            if (this.mostDown == badPoint)
                this.mostDown = p;
            if (p.X < this.mostLeft.X)
                this.mostLeft = p;
            if (p.X > this.mostRight.X)
                this.mostRight = p;
            if (p.Y > this.mostTop.Y)
                this.mostTop = p;
            if (p.Y < this.mostDown.Y)
                this.mostDown = p;
            this.PointList.Add(p);
        }
    }
}
