using Glyph.Entities;
using Glyph.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.UI
{
    static public class Cursor
    {
        static private readonly Sprite Sprite = new Sprite();
        static private bool _isMouseUsed;
        static public bool Enable { get; set; }
        static public Vector2 OriginCursor { get; set; }

        static public Vector2 Position
        {
            get { return Sprite.Position + OriginCursor; }
            set { Sprite.Position = value - OriginCursor; }
        }

        static public Point PositionSpace
        {
            get { return (Position / (Resolution.ScaleRatio * Camera.Zoom) + Camera.VectorPosition.XY()).ToPoint(); }
        }

        static public void Initialize(bool enable)
        {
            Enable = enable;

            Sprite.Initialize();
            Position = Resolution.Size / 2;
        }

        static public void LoadContent(ContentLibrary ressources, string asset)
        {
            Sprite.LoadContent(ressources, asset);
        }

        static public void HandleInput(InputManager input)
        {
            _isMouseUsed = input.IsMouseUsed;

            if (!Enable || !_isMouseUsed)
                return;

            Position = input.MouseWindow;
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            if (!Enable || !_isMouseUsed)
                return;

            Sprite.Draw(spriteBatch);
        }
    }
}