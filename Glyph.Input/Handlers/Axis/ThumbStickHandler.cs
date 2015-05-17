using System;
using Microsoft.Xna.Framework;

namespace Glyph.Input.Handlers.Axis
{
    public class ThumbStickHandler : AxisHandler
    {
        public PlayerIndex PlayerIndex { get; set; }
        public ThumbStick ThumbStick { get; set; }
        public Glyph.Axis Axis { get; set; }

        public override InputSource InputSource
        {
            get { return InputSource.GamePad; }
        }

        public ThumbStickHandler(string name, ThumbStick thumbStick, Glyph.Axis axis, PlayerIndex playerIndex = PlayerIndex.One)
            : base(name)
        {
            PlayerIndex = playerIndex;
            ThumbStick = thumbStick;
            Axis = axis;
        }

        protected override float GetState(InputManager inputManager)
        {
            Vector2 vector = ThumbStick == ThumbStick.Left
                ? inputManager.GamePadStates[PlayerIndex].ThumbSticks.Left
                : inputManager.GamePadStates[PlayerIndex].ThumbSticks.Right;

            return Axis == Glyph.Axis.Horizontal ? vector.X : vector.Y;
        }
    }
}