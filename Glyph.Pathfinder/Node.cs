using Microsoft.Xna.Framework;

namespace Glyph.Pathfinder
{
    public struct Node
    {
        public Point Parent { get; set; }
        public float ParentCost { get; set; }
        public float PersonalCost { get; set; }

        public float Cost
        {
            get { return ParentCost + PersonalCost; }
        }

        public Node Empty
        {
            get { return new Node(); }
        }

        public Node(Point p)
            : this()
        {
            Parent = p;
        }

        public override string ToString()
        {
            return "Cost : " + Cost;
        }
    }
}