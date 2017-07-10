using Glyph.Animation.Motors.Base;
using Microsoft.Xna.Framework;

namespace Glyph.Animation.Motors
{
    public class LinearMotor : MotorBase
    {
        public Vector2 Direction { get; set; }
        public float Speed { get; set; }

        public LinearMotor(Motion motion)
            : base(motion)
        {
        }

        protected override Vector2 UpdateVelocity(ElapsedTime elapsedTime)
        {
            return Direction * Speed * elapsedTime.GetDelta(Motion);
        }
    }
}