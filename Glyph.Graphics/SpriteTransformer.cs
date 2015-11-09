using Diese.Injection;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class SpriteTransformer : GlyphComponent
    {
        private ISpriteSource _spriteSource;
        private Vector2 _origin;
        private Vector2 _pivot;
        private AnchorType _lastAnchorType;
        public Color Color { get; set; }
        public Vector2 Scale { get; set; }
        public SpriteEffects Effects { get; set; }

        [Injectable]
        public ISpriteSource SpriteSource
        {
            get { return _spriteSource; }
            set
            {
                _spriteSource = value;
                RefreshAnchor();
            }
        }

        public Vector2 Origin
        {
            get { return _origin; }
            set
            {
                _origin = value;

                _lastAnchorType = AnchorType.Origin;
                RefreshAnchor();
            }
        }

        public Vector2 Pivot
        {
            get { return _pivot; }
            set
            {
                _pivot = value;

                _lastAnchorType = AnchorType.Pivot;
                RefreshAnchor();
            }
        }

        public float Opacity
        {
            get { return Color.A; }
            set { Color = new Color(Color.R, Color.G, Color.B, value); }
        }

        public SpriteTransformer()
        {
            Scale = Vector2.One;
            Color = Color.White;
            Pivot = Vector2.One * 0.5f;
        }

        private void RefreshAnchor()
        {
            Vector2 size = SpriteSource != null && SpriteSource.Texture != null
                ? SpriteSource.Rectangle != null
                    ? SpriteSource.Rectangle.Value.Size.ToVector2()
                    : SpriteSource.Texture.Size()
                : Vector2.Zero;

            switch (_lastAnchorType)
            {
                case AnchorType.Origin:
                    _pivot = size / _origin;
                    break;
                case AnchorType.Pivot:
                    _origin = size * _pivot;
                    break;
            }
        }

        private enum AnchorType
        {
            Origin,
            Pivot
        }
    }
}