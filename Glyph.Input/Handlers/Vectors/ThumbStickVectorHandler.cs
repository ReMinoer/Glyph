using System;
using Microsoft.Xna.Framework;

namespace Glyph.Input.Handlers.Vectors
{
    public class ThumbStickVectorHandler : VectorHandler
    {
        public PlayerIndex PlayerIndex { get; set; }
        public ThumbStick ThumbStick { get; set; }

        public override InputSource InputSource
        {
            get { return InputSource.GamePad; }
        }

        public ThumbStickVectorHandler()
            : this("", PlayerIndex.One, ThumbStick.None)
        {
        }

        public ThumbStickVectorHandler(string name, ThumbStick thumbStick, float deadZone = 0)
            : this("", PlayerIndex.One, thumbStick, deadZone)
        {
        }

        public ThumbStickVectorHandler(string name, PlayerIndex playerIndex, ThumbStick thumbStick, float deadZone = 0)
            : base(name, deadZone)
        {
            PlayerIndex = playerIndex;
            ThumbStick = thumbStick;
        }

        protected override Vector2 GetState(InputStates inputStates)
        {
            if (ThumbStick == ThumbStick.None)
                return Vector2.Zero;
            if (ThumbStick == ThumbStick.Left)
                return inputStates.GamePadStates[PlayerIndex].ThumbSticks.Left;
            if (ThumbStick == ThumbStick.Right)
                return inputStates.GamePadStates[PlayerIndex].ThumbSticks.Right;

            throw new NotImplementedException();
        }
    }
}