using Microsoft.Xna.Framework;

namespace Glyph.Animation.Trajectories
{
    public abstract class StandardTrajectory : IStandardTrajectory
    {
        public abstract Vector2 Destination { get; }
        public abstract float Length { get; }
        public abstract Vector2 GetPosition(float advance);

        public ITimedTrajectory AsTimed(float duration)
        {
            return new Timed(this, duration);
        }

        public ITimedTrajectory AsTimed(float duration, EasingDelegate easing)
        {
            return new Timed(this, duration, easing);
        }

        public IProgressiveTrajectory AsProgressive()
        {
            return new Progressive(this);
        }

        public class Timed : ITimedTrajectory
        {
            public StandardTrajectory StandardTrajectory { get; private set; }
            public float Duration { get; private set; }
            public EasingDelegate Easing { get; private set; }

            public Vector2 Destination
            {
                get { return StandardTrajectory.Destination; }
            }

            public Timed(StandardTrajectory standardTrajectory, float duration)
            {
                StandardTrajectory = standardTrajectory;
                Duration = duration;
            }

            public Timed(StandardTrajectory standardTrajectory, float duration, EasingDelegate easing)
                : this(standardTrajectory, duration)
            {
                Easing = easing;
            }

            public Vector2 GetPosition(float time, out float advance)
            {
                time = MathHelper.Clamp(time, 0, Duration);

                float rawAdvance = Duration <= 0 ? 1 : time / Duration;
                advance = Easing(rawAdvance);

                return StandardTrajectory.GetPosition(advance);
            }
        }

        public class Progressive : IProgressiveTrajectory
        {
            public StandardTrajectory StandardTrajectory { get; private set; }

            public Vector2 Destination
            {
                get { return StandardTrajectory.Destination; }
            }

            public float Length
            {
                get { return StandardTrajectory.Length; }
            }

            public Progressive(StandardTrajectory standardTrajectory)
            {
                StandardTrajectory = standardTrajectory;
            }

            public Vector2 GetPosition(float distance, out float advance)
            {
                distance = MathHelper.Clamp(distance, 0, Length);

                advance = Length <= 0 ? 1 : distance / Length;

                return StandardTrajectory.GetPosition(advance);
            }
        }
    }
}