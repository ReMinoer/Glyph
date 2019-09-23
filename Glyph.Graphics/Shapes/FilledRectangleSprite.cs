using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Shapes
{
    public class FilledRectangleSprite : ShapedSpriteBase
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Point Size
        {
            get => new Point(Width, Height);
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        public FilledRectangleSprite(Func<GraphicsDevice> graphicsDeviceFunc)
            : base(graphicsDeviceFunc)
        {
            Width = 100;
            Height = 100;
        }

        protected override Task<Texture2D> GenerateTexture()
        {
            return Task.Run(() =>
            {
                var data = new Color[Width * Height];
                for (int i = 0; i < data.Length; i++)
                    data[i] = Color;

                var texture = new Texture2D(GraphicsDeviceFunc(), Width, Height);
                texture.SetData(data);
                return texture;
            });
        }
    }
}