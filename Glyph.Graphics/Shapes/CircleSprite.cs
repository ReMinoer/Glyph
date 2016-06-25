using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Shapes
{
    public class CircleSprite : ShapedSpriteBase
    {
        public int Radius { get; set; }

        public CircleSprite(Lazy<GraphicsDevice> lazyGraphicsDevice)
            : base(lazyGraphicsDevice)
        {
            Radius = 50;
        }

        public override void GenerateTexture()
        {
            int outerRadius = (Radius + 1) * 2;

            var data = new Color[outerRadius * outerRadius];

            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / Radius;

            for (double angle = 0; angle < System.Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                int x = (int)System.Math.Round(Radius + Radius * System.Math.Cos(angle));
                int y = (int)System.Math.Round(Radius + Radius * System.Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color;
            }

            _texture = new Texture2D(LazyGraphicsDevice.Value, outerRadius, outerRadius);
            Texture.SetData(data);
        }
    }
}