using Glyph.Composition;
using Glyph.Core;
using Microsoft.Xna.Framework;
using Niddle.Attributes;

namespace Glyph.Animation.Parallax
{
    public class ParallaxLayer : GlyphComponent, IUpdate
    {
        private readonly SceneNode _sceneNode;

        [Resolvable]
        public ParallaxRoot ParallaxRoot { get; set; }

        public Vector2 StartLocalPosition { get; private set; }
        public Vector2 LocalPosition
        {
            get => _sceneNode.LocalPosition;
            set => _sceneNode.LocalPosition = value;
        }

        public float Depth => _sceneNode.Depth;

        public ParallaxLayer(SceneNode sceneNode)
        {
            _sceneNode = sceneNode;
        }

        public override void Initialize()
        {
            base.Initialize();
            StartLocalPosition = LocalPosition;
        }

        public void Update(ElapsedTime elapsedTime)
        {
            ParallaxRoot.UpdateLayer(this);
        }
    }
}