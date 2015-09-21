using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Shapes
{
    public class FilledRectangleSprite : ShapedSpriteBase
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public FilledRectangleSprite(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            Width = 100;
            Height = 100;
        }

        public override void LoadContent(ContentLibrary contentLibrary)
        {
            var data = new Color[Width * Height];
            for (int i = 0; i < data.Length; i++)
                data[i] = Color;

            Texture = new Texture2D(GraphicsDevice, Width, Height);
            Texture.SetData(data);
        }
    }
}