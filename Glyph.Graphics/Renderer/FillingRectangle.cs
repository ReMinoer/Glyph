using Glyph.Composition;
using Glyph.Core;
using Glyph.Math;
using Glyph.Math.Shapes;

namespace Glyph.Graphics.Renderer
{
    public class FillingRectangle : GlyphComponent, IShapedComponent
    {
        public ISceneNodeComponent SceneNode { get; }
        public Quad LocalRectangle { get; set; }

        public Quad Rectangle
        {
            get => SceneNode.Transform(LocalRectangle);
            set => LocalRectangle = SceneNode.InverseTransform(value);
        }

        IArea IBoxedComponent.Area => Rectangle;
        IShape IShapedComponent.Shape => Rectangle;

        public FillingRectangle(ISceneNodeComponent sceneNode)
        {
            SceneNode = sceneNode;
        }
    }
}