using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class TargetView : TargetViewBase
    {
        new public Vector2 Size
        {
            get => base.Size;
            set => base.Size = value;
        }

        public TargetView(Func<GraphicsDevice> graphicsDeviceFunc)
            : base(graphicsDeviceFunc)
        {
        }
    }
}