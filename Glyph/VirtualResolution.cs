using System;
using Microsoft.Xna.Framework;

namespace Glyph
{
    static public class VirtualResolution
    {
        static private Vector2 _size = new Vector2(800, 450);
        static public float AspectRatio => Size.X / Size.Y;

        static public Vector2 Size
        {
            get { return _size; }
            set
            {
                if (_size == value)
                    return;

                _size = value;
                SizeChanged?.Invoke(_size);
            }
        }

        static public event Action<Vector2> SizeChanged;
    }
}