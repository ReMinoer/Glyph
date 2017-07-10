using Glyph.Composition;
using Microsoft.Xna.Framework;

namespace Glyph.Animation.Motors.Base
{
    public abstract class MotorBase : GlyphComponent, IUpdate, IEnableable, ITimeUnscalable
    {
        public bool Enabled { get; set; } = true;
        public Motion Motion { get; }
        public Vector2 Velocity { get; private set; }
        public bool UseUnscaledTime => Motion.UseUnscaledTime;

        protected MotorBase(Motion motion)
        {
            Motion = motion;
            Motion.RegisterMotor(this);
        }

        public void Update(ElapsedTime elapsedTime)
        {
            Velocity = UpdateVelocity(elapsedTime);
        }

        protected abstract Vector2 UpdateVelocity(ElapsedTime elapsedTime);

        public override void Dispose()
        {
            Motion.UnregisterMotor(this);
        }
    }
}