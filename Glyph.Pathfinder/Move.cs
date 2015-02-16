using Microsoft.Xna.Framework;

namespace Glyph.Pathfinder
{
    public abstract class Move<TAction>
    {
        public Point Destination { get { return new Point(Point.X + Modif.X, Point.Y + Modif.Y); } }
        public Point Point { get; protected set; }
        public Point Modif { get; protected set; }
        public abstract TAction Action { get; }

        protected Move(Point p, Point m)
        {
            Point = p;
            Modif = m;
        }

        public override string ToString()
        {
            return Action + " " + Destination;
        }
    }
}