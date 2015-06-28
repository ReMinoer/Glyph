using Microsoft.Xna.Framework;

namespace Glyph.Animation.Trajectories
{
    public interface IStandardTrajectory : ITrajectory
    {
        float Length { get; }
        Vector2 GetPosition(float advance);
        ITimedTrajectory AsTimed(float duration);
        ITimedTrajectory AsTimed(float duration, EasingDelegate easing);
        IProgressiveTrajectory AsProgressive();
    }
}