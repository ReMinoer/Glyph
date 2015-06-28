using Microsoft.Xna.Framework;

namespace Glyph.Animation.Trajectories
{
    public interface IProgressiveTrajectory : ITrajectory
    {
        float Length { get; }
        Vector2 GetPosition(float distance, out float advance);
    }
}