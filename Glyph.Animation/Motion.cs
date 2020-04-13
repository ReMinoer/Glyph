using System.Collections.Generic;
using System.Linq;
using Glyph.Animation.Motors.Base;
using Glyph.Composition;
using Glyph.Core;
using Microsoft.Xna.Framework;

namespace Glyph.Animation
{
    // TODO : Splines
    // TODO : Space unit length (pixel to meter)
    [SinglePerParent]
    public class Motion : GlyphComponent, IUpdate, ITimeUnscalable
    {
        private readonly List<MotorBase> _motors = new List<MotorBase>();
        public bool UseUnscaledTime { get; set; }
        public SceneNode SceneNode { get; }

        public Motion(SceneNode sceneNode)
        {
            SceneNode = sceneNode;
        }

        internal void RegisterMotor(MotorBase motor) => _motors.Add(motor);
        internal void UnregisterMotor(MotorBase motor) => _motors.Remove(motor);

        public void Update(ElapsedTime elapsedTime)
        {
            SceneNode.LocalPosition += _motors.Where(x => x.Active).Aggregate(Vector2.Zero, (current, motor) => current + motor.Velocity);
        }
    }
}