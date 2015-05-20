using Microsoft.Xna.Framework;

namespace Glyph.Input.Handlers.Axis
{
    public class ThumbStickAxisHandler : AxisHandler
    {
        public PlayerIndex PlayerIndex { get; set; }
        public ThumbStick ThumbStick { get; set; }
        public Glyph.Axis Axis { get; set; }

        public override InputSource InputSource
        {
            get { return InputSource.GamePad; }
        }

        public ThumbStickAxisHandler(string name, ThumbStick thumbStick, Glyph.Axis axis,
            PlayerIndex playerIndex = PlayerIndex.One)
            : base(name)
        {
            PlayerIndex = playerIndex;
            ThumbStick = thumbStick;
            Axis = axis;
        }

        protected override float GetState(InputStates inputStates)
        {
            Vector2 vector = ThumbStick == ThumbStick.Left
                ? inputStates.GamePadStates[PlayerIndex].ThumbSticks.Left
                : inputStates.GamePadStates[PlayerIndex].ThumbSticks.Right;

            return Axis == Glyph.Axis.Horizontal ? vector.X : vector.Y;
        }
    }
}