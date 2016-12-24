﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Shapes
{
    public class CircleSprite : ShapedSpriteBase
    {
        public int Radius { get; set; }

        public CircleSprite(Func<GraphicsDevice> graphicsDeviceFunc)
            : base(graphicsDeviceFunc)
        {
            Radius = 50;
        }

        protected override void GenerateTexture()
        {
            int outerDiameter = (Radius + 1) * 2;

            var data = new Color[outerDiameter * outerDiameter];

            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / Radius;

            for (double angle = 0; angle < System.Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                int x = (int)System.Math.Round(Radius + Radius * System.Math.Cos(angle));
                int y = (int)System.Math.Round(Radius + Radius * System.Math.Sin(angle));

                data[y * outerDiameter + x + 1] = Color;
            }

            _texture = new Texture2D(GraphicsDeviceFunc(), outerDiameter, outerDiameter);
            Texture.SetData(data);
        }
    }
}