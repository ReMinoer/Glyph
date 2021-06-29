using Glyph.Composition;
using Glyph.Core;
using Microsoft.Xna.Framework;

namespace Glyph.Animation.Parallax
{
    public class ParallaxRoot : GlyphComponent
    {
        public SceneNode Referential { get; }
        public ISceneNode PointOfView { get; set; }
        public float ParallaxCoefficient { get; set; } = 1;

        public ParallaxRoot(SceneNode sceneNode)
        {
            Referential = sceneNode;
        }

        public void UpdateLayer(ParallaxLayer layer)
        {
            float depthDifference = layer.Depth - Referential.Depth;
            if (depthDifference.EqualsZero())
                return;

            Vector2 pointOfViewDifference = PointOfView.Position - Referential.Position;
            layer.LocalPosition = layer.StartLocalPosition + pointOfViewDifference * depthDifference * ParallaxCoefficient;
        }
    }
}