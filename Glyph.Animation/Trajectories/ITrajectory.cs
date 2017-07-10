using Microsoft.Xna.Framework;

namespace Glyph.Animation.Trajectories
{
    public interface ITrajectory
    {
        Vector2 Origin { get; }
        Vector2 Destination { get; }
        Vector2 GetPosition(float progress);
    }
}