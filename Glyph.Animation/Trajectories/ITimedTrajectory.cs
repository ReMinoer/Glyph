using Microsoft.Xna.Framework;

namespace Glyph.Animation.Trajectories
{
    public interface ITimedTrajectory : ITrajectory
    {
        float Duration { get; }
        float GetTime(float progress);
        Vector2 GetPosition(float progress, out float time);
        Vector2 GetPositionAtTime(float time);
        Vector2 GetPositionAtTime(float time, out float progress);
    }
}