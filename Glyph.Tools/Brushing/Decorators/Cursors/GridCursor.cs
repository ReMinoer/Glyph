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
        private readonly TriangulableShapePrimitive<Quad> _cursorPrimitive;

        public Quad Rectangle
        {
            get => _cursorPrimitive.Shape;
            set
            {
                _sceneNode.Position = value.P0;
                _cursorPrimitive.Shape = new Quad(Vector2.Zero, value.P1 - value.P0, value.P2 - value.P0);
            }
        }

        public GridCursor(GlyphResolveContext context)
            : base(context)
        {
            _sceneNode = Add<SceneNode>();
            _cursorPrimitive = new Quad().ToPrimitive(Color.White * 0.5f);

            Add<PrimitiveRenderer>().PrimitiveProviders.Add(_cursorPrimitive);
        }
    }
}