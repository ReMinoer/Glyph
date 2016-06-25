using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Shapes
{
    public class FilledRectangleSprite : ShapedSpriteBase
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public FilledRectangleSprite(Lazy<GraphicsDevice> lazyGraphicsDevice)
            : base(lazyGraphicsDevice)
        {
            Width = 100;
            Height = 100;
        }

        public override void GenerateTexture()
        {
            var data = new Color[Width * Height];
            for (int i = 0; i < data.Length; i++)
                data[i] = Color;

            _texture = new Texture2D(LazyGraphicsDevice.Value, Width, Height);
            Texture.SetData(data);
        }
    }
}