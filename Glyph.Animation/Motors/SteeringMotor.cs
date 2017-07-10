using Glyph.Animation.Motors.Base;
using Glyph.Core;
using Microsoft.Xna.Framework;

namespace Glyph.Animation.Motors
{
    public class SteeringMotor : MotorBase
    {
        private Vector2 _direction;
        public float Speed { get; set; }
        public float AngularSpeed { get; set; } = float.MaxValue;
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

        public SteeringMotor(Motion motion)
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
                _direction = Vector2.Zero;
                return Vector2.Zero;
            }

            if (Speed.EqualsZero())
                return Vector2.Zero;

            if (_direction.EqualsZero())
                _direction = remainingDistance.Normalized();
            else
                _direction = _direction.RotateToward(remainingDistance, AngularSpeed, elapsedTime.GetDelta(Motion)).Normalized();

            Vector2 velocity = _direction * Speed * elapsedTime.GetDelta(Motion);
            if (float.IsPositiveInfinity(velocity.X) || float.IsNegativeInfinity(velocity.X) || float.IsPositiveInfinity(velocity.Y) || float.IsNegativeInfinity(velocity.Y))
                return remainingDistance;
            
            Vector2 newRemainingDistance = Destination.Value - (Motion.SceneNode.Position + velocity);
            return Vector2.Dot(newRemainingDistance, remainingDistance) <= 0 ? remainingDistance : velocity;
        }

        public void Stop()
        {
            _direction = Vector2.Zero;
        }
    }
}