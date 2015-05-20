using Microsoft.Xna.Framework;

namespace Glyph.Input.Handlers.Vectors
{
    public class ThumbStickVectorHandler : VectorHandler
    {
        public PlayerIndex PlayerIndex { get; private set; }
        public ThumbStick ThumbStick { get; private set; }

        public override InputSource InputSource
        {
            get { return InputSource.GamePad; }
        }

        public ThumbStickVectorHandler(string name, ThumbStick thumbStick, float deadZone = 0)
            : this(name, PlayerIndex.One, thumbStick, deadZone)
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
            return ThumbStick == ThumbStick.Right
                ? inputStates.GamePadStates[PlayerIndex].ThumbSticks.Right
                : inputStates.GamePadStates[PlayerIndex].ThumbSticks.Left;
        }
    }
}