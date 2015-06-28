using Microsoft.Xna.Framework;

namespace Glyph.Animation.Trajectories
{
    public interface ITimedTrajectory : ITrajectory
    {
        float Duration { get; }
        EasingDelegate Easing { get; }
        Vector2 GetPosition(float time, out float advance);
    }
}