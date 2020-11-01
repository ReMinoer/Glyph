using Glyph.Core;
using Glyph.Graphics.Primitives;
using Glyph.Graphics.Renderer;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Brushing.Decorators.Cursors
{
    public class GridCursor : GlyphObject
    {
        private readonly SceneNode _sceneNode;
        private readonly TriangulableShapePrimitive<TopLeftRectangle> _cursorPrimitive;

        public TopLeftRectangle Rectangle
        {
            get => _cursorPrimitive.Shape;
            set
            {
                _sceneNode.Position = value.Position;
                _cursorPrimitive.Shape = new TopLeftRectangle(Vector2.Zero, value.Size);
            }
        }

        public GridCursor(GlyphResolveContext context)
            : base(context)
        {
            _sceneNode = Add<SceneNode>();
            _cursorPrimitive = new TopLeftRectangle().ToPrimitive(Color.White * 0.5f);

            Add<PrimitiveRenderer>().PrimitiveProviders.Add(_cursorPrimitive);
        }
    }
}