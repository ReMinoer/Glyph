using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Shapes
{
    public class CircleSprite : ShapedSpriteBase
    {
        public int Radius { get; set; } = 50;
        public int Thickness { get; set; } = 1;

        public CircleSprite(Func<GraphicsDevice> graphicsDeviceFunc)
            : base(graphicsDeviceFunc)
        {
        }

        protected override Task<Texture2D> GenerateTexture()
        {
            return Task.Run(() =>
            {
                int outerDiameter = (Radius + 1) * 2;

                var data = new Color[outerDiameter * outerDiameter];

                for (int i = 0; i < data.Length; i++)
                    data[i] = Color.Transparent;

                for (int i = 0; i < Thickness; i++)
                {
                    // Work out the minimum step necessary using trigonometry + sine approximation.
                    double angleStep = 1f / (Radius - i);

                    for (double angle = 0; angle < System.Math.PI * 2; angle += angleStep)
                    {
                        // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                        int x = (int)System.Math.Round(Radius + (Radius - i) * System.Math.Cos(angle));
                        int y = (int)System.Math.Round(Radius + (Radius - i) * System.Math.Sin(angle));

                        data[y * outerDiameter + x + 1] = Color;
                    }
                }

                var texture = new Texture2D(GraphicsDeviceFunc(), outerDiameter, outerDiameter);
                texture.SetData(data);
                return texture;
            });
        }
    }
}