using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class SpriteDescriptor : GlyphComponent, ILoadContent
    {
        private enum AnchorType
        {
            Origin,
            Pivot
        }

        private Texture2D _texture;
        private Vector2 _origin;
        private Vector2 _pivot;
        private AnchorType _lastAnchorType;

        public string Asset { get; set; }
        public Rectangle? SourceRectangle { get; set; }
        public Color Color { get; set; }
        public SpriteEffects Effects { get; set; }

        public Texture2D Texture
        {
            get { return _texture; }
            set
            {
                _texture = value;

                if (Texture == null)
                    SourceRectangle = null;
                else if (SourceRectangle == null)
                    SourceRectangle = Texture.Bounds;

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

        public SpriteDescriptor()
        {
            Color = Color.White;
            Pivot = Vector2.One * 0.5f;
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            Texture = contentLibrary.GetTexture(Asset);
        }

        public override void Dispose()
        {
            base.Dispose();

            Texture.Dispose();
        }

        private void RefreshAnchor()
        {
            switch (_lastAnchorType)
            {
                case AnchorType.Origin:
                    _pivot = Texture.Size() / _origin;
                    break;
                case AnchorType.Pivot:
                    _origin = Texture.Size() * _pivot;
                    break;
            }
        }
    }
}