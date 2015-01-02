using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    static public class Texture2DExtension
    {
        static public Vector2 Size(this Texture2D value)
        {
            return new Vector2(value.Width, value.Height);
        }

        static public Rectangle Hitbox(this Texture2D value, Vector2 pos)
        {
            return new Rectangle((int)pos.X, (int)pos.Y, value.Width, value.Height);
        }
    }
}