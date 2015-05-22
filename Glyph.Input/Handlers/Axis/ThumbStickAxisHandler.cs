using System;
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

        public ThumbStickAxisHandler()
            : this("", ThumbStick.None, Glyph.Axis.None)
        {
        }

        public ThumbStickAxisHandler(string name, ThumbStick thumbStick, Glyph.Axis axis)
            : this(name, PlayerIndex.One, thumbStick, axis)
        {
        }

        public ThumbStickAxisHandler(string name, PlayerIndex playerIndex, ThumbStick thumbStick, Glyph.Axis axis)
            : base(name)
        {
            PlayerIndex = playerIndex;
            ThumbStick = thumbStick;
            Axis = axis;
        }

        protected override float GetState(InputStates inputStates)
        {
            Vector2 vector;
            switch (ThumbStick)
            {
                case ThumbStick.None:
                    return 0;
                case ThumbStick.Left:
                    vector = inputStates.GamePadStates[PlayerIndex].ThumbSticks.Left;
                    break;
                case ThumbStick.Right:
                    vector = inputStates.GamePadStates[PlayerIndex].ThumbSticks.Right;
                    break;
                default:
                    throw new NotImplementedException();
            }

            switch (Axis)
            {
                case Glyph.Axis.None:
                    return 0;
                case Glyph.Axis.Horizontal:
                    return vector.X;
                case Glyph.Axis.Vertical:
                    return vector.Y;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}