using Glyph.Math.Shapes;

namespace Glyph.Space.Partitioning
{
    public class SpacePartitionerParameters
    {
        public int NodeCapacity { get; private set; }
        public int DepthMax { get; private set; }
        public IRectangle Bounds { get; private set; }

        public SpacePartitionerParameters(int nodeCapacity, int depthMax, IRectangle bounds)
        {
            NodeCapacity = nodeCapacity;
            DepthMax = depthMax;
            Bounds = bounds;
        }
    }
}