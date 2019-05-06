using Glyph.Composition;
using Glyph.Resolver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Niddle.Attributes;

namespace Glyph.Graphics
{
    public class SpriteTransformer : GlyphComponent, IFlipable
    {
        private ISpriteSource _spriteSource;
        private Vector2 _origin = new Vector2(float.NaN, float.NaN);
        private Vector2 _pivot = new Vector2(float.NaN, float.NaN);
        private AnchorType _lastAnchorType;
        private bool _spriteSizeKnown;
        public Color Color { get; set; }
        public Vector2 Scale { get; set; }
        public Axes FlipAxes { get; set; }

        public SpriteEffects Effects
        {
            get
            {
                var effects = SpriteEffects.None;
                if ((FlipAxes & Axes.Horizontal) == Axes.Horizontal)
                    effects |= SpriteEffects.FlipHorizontally;
                if ((FlipAxes & Axes.Vertical) == Axes.Vertical)
                    effects |= SpriteEffects.FlipVertically;
                return effects;
            }
        }

        [Resolvable, ResolveTargets(ResolveTargets.Fraternal)]
        public ISpriteSource SpriteSource
        {
            get { return _spriteSource; }
            set
            {
                _spriteSource = value;

                if (_spriteSource != null)
                    _spriteSource.Loaded += x => RefreshAnchor();

                RefreshAnchor();
            }
        }

        public Vector2 Origin
        {
            get
            {
                if (!_spriteSizeKnown)
                    RefreshAnchor();
                
                return _origin;
            }
            set
            {
                _origin = value;

                _lastAnchorType = AnchorType.Origin;
                RefreshAnchor();
            }
        }

        public Vector2 Pivot
        {
            get
            {
                if (!_spriteSizeKnown)
                    RefreshAnchor();
                
                return _pivot;
            }
            set
            {
                _pivot = value;

                _lastAnchorType = AnchorType.Pivot;
                RefreshAnchor();
            }
        }

        public float Opacity
        {
            get { return Color.GetOpacity(); }
            set { Color = Color.WithOpacity(value); }
        }

        public SpriteTransformer()
        {
            Scale = Vector2.One;
            Color = Color.White;
            Pivot = Vector2.One * 0.5f;
        }

        public void Flip(Axes axes)
        {
            FlipAxes ^= axes;
        }

        private void RefreshAnchor()
        {
            if (SpriteSource?.Texture == null)
                return;

            Vector2 size = SpriteSource.Rectangle?.Size.ToVector2() ?? SpriteSource.Texture.Size();
            _spriteSizeKnown = true;

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