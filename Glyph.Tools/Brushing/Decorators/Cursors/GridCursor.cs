using Glyph.Core;
using Glyph.Graphics.Meshes;
using Glyph.Graphics.Renderer;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Brushing.Decorators.Cursors
{
    public class GridCursor : GlyphObject
    {
        private readonly SceneNode _sceneNode;
        private readonly TriangulableShapeMesh<Quad> _cursorMesh;

        public Quad Rectangle
        {
            get => _cursorMesh.Shape;
            set
            {
                _sceneNode.Position = value.P0;
                _cursorMesh.Shape = new Quad(Vector2.Zero, value.P1 - value.P0, value.P2 - value.P0);
            }
        }

        public GridCursor(GlyphResolveContext context)
            : base(context)
        {
            _sceneNode = Add<SceneNode>();
            _cursorMesh = new Quad().ToMesh(Color.White * 0.5f);

            Add<MeshRenderer>().MeshProviders.Add(_cursorMesh);
        }
    }
}