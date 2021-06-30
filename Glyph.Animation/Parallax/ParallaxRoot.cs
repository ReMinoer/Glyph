using Glyph.Composition;
using Glyph.Core;
using Microsoft.Xna.Framework;

namespace Glyph.Animation.Parallax
{
    public class ParallaxRoot : GlyphComponent
    {
        public SceneNode Referential { get; }
        public ISceneNode PointOfView { get; set; }
        public float MinimumDepth { get; set; } = -10;

        public ParallaxRoot(SceneNode sceneNode)
        {
            Referential = sceneNode;
        }

        public void UpdateLayer(ParallaxLayer layer)
        {
            float layerDepth = layer.Depth;
            if (layerDepth < MinimumDepth)
                layerDepth = MinimumDepth;

            float depthDifference = layerDepth - Referential.Depth;
            if (depthDifference.EqualsZero())
                return;

            float minDepthDifference = MinimumDepth - Referential.Depth;
            float parallaxCoefficient = depthDifference / minDepthDifference;
            
            Vector2 pointOfViewDifference = PointOfView.Position - Referential.Position;
            layer.LocalPosition = layer.StartLocalPosition + pointOfViewDifference * parallaxCoefficient;
        }
    }
}