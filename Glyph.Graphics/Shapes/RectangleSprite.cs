﻿using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Shapes
{
    public class RectangleSprite : ShapedSpriteBase
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

        public RectangleSprite(Func<GraphicsDevice> graphicsDeviceFunc)
            : base(graphicsDeviceFunc)
        {
            Width = 100;
            Height = 100;
        }

        protected override Texture2D GenerateTexture()
        {
            var data = new Color[Width * Height];
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            for (int i = 0; i < Width; i++)
            {
                data[i] = Color;
                data[(Width - 1) * Width + i] = Color;
            }

            for (int i = 0; i < Height; i++)
            {
                data[i * Width] = Color;
                data[i * Width + (Width - 1)] = Color;
            }

            var texture = new Texture2D(GraphicsDeviceFunc(), Width, Height);
            texture.SetData(data);
            return texture;
        }
    }
}