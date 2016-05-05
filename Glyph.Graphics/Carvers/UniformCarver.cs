using System;
using Microsoft.Xna.Framework;

namespace Glyph.Graphics.Carvers
{
    public class UniformCarver : ISpriteSheetCarver
    {
        public int Columns { get; set; }
        public int Rows { get; set; }
        public int Margin { get; set; }

        public UniformCarver(int columns, int rows, int margin = 0)
        {
            if (columns < 1)
                throw new ArgumentOutOfRangeException(nameof(columns));
            if (rows < 1)
                throw new ArgumentOutOfRangeException(nameof(rows));

            Columns = columns;
            Rows = rows;
            Margin = margin;
        }

        public void Process(SpriteSheet spriteSheet)
        {
            int width = spriteSheet.Texture.Width / Columns - 2 * Margin;
            int height = spriteSheet.Texture.Height / Rows - 2 * Margin;

            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                {
                    var frame = new Rectangle
                    {
                        X = c * spriteSheet.Texture.Width / Columns + Margin,
                        Y = r * spriteSheet.Texture.Height / Rows + Margin,
                        Width = width,
                        Height = height
                    };

                    spriteSheet.Frames.Add(frame);
                }
        }
    }
}