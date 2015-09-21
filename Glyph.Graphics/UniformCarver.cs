using System;
using Microsoft.Xna.Framework;

namespace Glyph.Graphics
{
    public class UniformCarver : ISpriteSheetCarver
    {
        public int Columns { get; set; }
        public int Rows { get; set; }

        public UniformCarver(int columns, int rows)
        {
            if (columns < 1)
                throw new ArgumentOutOfRangeException("columns");
            if (rows < 1)
                throw new ArgumentOutOfRangeException("rows");

            Columns = columns;
            Rows = rows;
        }

        public void Process(SpriteSheet spriteSheet)
        {
            int width = spriteSheet.Texture.Width / Columns;
            int height = spriteSheet.Texture.Height / Rows;

            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                {
                    var frame = new Rectangle
                    {
                        X = c * width,
                        Y = r * height,
                        Width = width,
                        Height = height
                    };

                    spriteSheet.Frames.Add(frame);
                }
        }
    }
}