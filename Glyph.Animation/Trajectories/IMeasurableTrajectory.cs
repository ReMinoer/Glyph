using Microsoft.Xna.Framework;

namespace Glyph.Animation.Trajectories
{
    public interface IMeasurableTrajectory : ITrajectory
    {
        float Length { get; }
        Vector2 GetPositionAtDistance(float distance);
        Vector2 GetPositionAtDistance(float distance, out float progress);
    }
}