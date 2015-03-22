namespace Glyph
{
    public struct Checkpoint
    {
        private int _layer;
        private int _x;
        private int _y;

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public int Layer
        {
            get { return _layer; }
            set { _layer = value; }
        }

        public Checkpoint(int x, int y, int l)
        {
            _x = x;
            _y = y;
            _layer = l;
        }
    }
}