using Microsoft.Xna.Framework;

namespace Glyph.Animation.Trajectories
{
    public interface IMeasurableTrajectory : ITrajectory
    {
        float Length { get; }
        float GetDistance(float progress);
        Vector2 GetPosition(float progress, out float distance);
        Vector2 GetPositionAtDistance(float distance);
        Vector2 GetPositionAtDistance(float distance, out float progress);
    }
}