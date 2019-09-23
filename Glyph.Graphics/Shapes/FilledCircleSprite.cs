using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Shapes
{
    public class FilledCircleSprite : ShapedSpriteBase
    {
        public int Radius { get; set; }

        public FilledCircleSprite(Func<GraphicsDevice> graphicsDeviceFunc)
            : base(graphicsDeviceFunc)
        {
            Radius = 50;
        }

        protected override Task<Texture2D> GenerateTexture()
        {
            return Task.Run(() =>
            {
                int outerDiameter = (Radius + 1) * 2;

                var data = new Color[outerDiameter * outerDiameter];
            
                for (int i = 0; i < outerDiameter; i++)
                    for (int j = 0; j < outerDiameter; j++)
                    {
                        Vector2 position = new Vector2(j, i).Substract(Radius);
                        data[i * outerDiameter + j] = position.Length() <= Radius ? Color : Color.Transparent;
                    }

                var texture = new Texture2D(GraphicsDeviceFunc(), outerDiameter, outerDiameter);
                texture.SetData(data);
                return texture;
            });
        }
    }
}