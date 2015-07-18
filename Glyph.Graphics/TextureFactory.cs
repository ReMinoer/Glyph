using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class TextureFactory
    {
        private readonly GraphicsDevice _graphicsDevice;

        public TextureFactory(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public Texture2D CreateRectangle(int width, int height, Color color)
        {
            var texture = new Texture2D(_graphicsDevice, width, height);

            var data = new Color[width * height];
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            for (int i = 0; i < width; i++)
            {
                data[i] = color;
                data[(width - 1) * width + i] = color;
            }
            for (int i = 0; i < height; i++)
            {
                data[i * width] = color;
                data[i * width + (width - 1)] = color;
            }

            texture.SetData(data);
            return texture;
        }

        public Texture2D CreateFilledRectangle(int width, int height, Color color)
        {
            var texture = new Texture2D(_graphicsDevice, width, height);

            var data = new Color[width * height];
            for (int i = 0; i < data.Length; i++)
                data[i] = color;

            texture.SetData(data);
            return texture;
        }

        public Texture2D CreateCircle(int radius, Color color)
        {
            int outerRadius = (radius + 1) * 2;
            var texture = new Texture2D(_graphicsDevice, outerRadius, outerRadius);

            var data = new Color[outerRadius * outerRadius];

            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                int y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = color;
            }

            texture.SetData(data);
            return texture;
        }
    }
}