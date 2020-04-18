using Glyph.Animation.Motors.Base;
using Microsoft.Xna.Framework;

namespace Glyph.Animation.Motors
{
    public class SeekingMotor : MotorBase
    {
        private Vector2 _velocity;
        public float Speed { get; set; }
        public float AccelerationMax { get; set; } = float.MaxValue;
        public ISceneNode Target { get; set; }

        private Vector2? _destination;
        public Vector2? Destination
        {
            get => Target?.Position ?? _destination;
            set
            {
                _destination = value;
                Target = null;
            }
        }

        public SeekingMotor(Motion motion)
            : base(motion)
        {
        }

        protected override Vector2 UpdateVelocity(ElapsedTime elapsedTime)
        {
            if (Destination == null)
                return Vector2.Zero;

            Vector2 remainingDistance = Destination.Value - Motion.SceneNode.Position;
            if (remainingDistance.EqualsZero())
            {
                _velocity = Vector2.Zero;
                return Vector2.Zero;
            }

            if (Speed.EqualsZero())
                return Vector2.Zero;
            
            Vector2 desiredVelocity = remainingDistance.Normalized() * Speed * elapsedTime.GetDelta(Motion);
            Vector2 acceleration = desiredVelocity - _velocity;
            if (acceleration.Length() > AccelerationMax)
                acceleration = acceleration.Normalized() * AccelerationMax;

            _velocity += acceleration;
            if (_velocity.Length() > Speed)
                _velocity = _velocity.Normalized() * Speed;

            if (float.IsPositiveInfinity(_velocity.X) || float.IsNegativeInfinity(_velocity.X) || float.IsPositiveInfinity(_velocity.Y) || float.IsNegativeInfinity(_velocity.Y))
                return remainingDistance;

            Vector2 newRemainingDistance = Destination.Value - (Motion.SceneNode.Position + _velocity);
            return Vector2.Dot(newRemainingDistance, remainingDistance) <= 0 ? remainingDistance : _velocity;
        }

        public void Stop()
        {
            _velocity = Vector2.Zero;
        }
    }
}