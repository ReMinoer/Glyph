namespace Glyph.UI
{
    public struct Padding
    {
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }

        public Padding(int value)
            : this()
        {
            Top = value;
            Right = value;
            Bottom = value;
            Left = value;
        }

        public Padding(int top, int right, int bottom, int left)
            : this()
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }
    }
}