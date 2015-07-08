using Diese.Injection;
using Microsoft.Xna.Framework;

namespace Glyph
{
    public class GameObject2D : GlyphObject
    {
        public SceneNode SceneNode { get; private set; }

        public Vector2 Position
        {
            get { return SceneNode.Position; }
            set { SceneNode.Position = value; }
        }

        public float Rotation
        {
            get { return SceneNode.Rotation; }
            set { SceneNode.Rotation = value; }
        }

        public float Scale
        {
            get { return SceneNode.Scale; }
            set { SceneNode.Scale = value; }
        }

        public Vector2 LocalPosition
        {
            get { return SceneNode.LocalPosition; }
            set { SceneNode.LocalPosition = value; }
        }

        public float LocalRotation
        {
            get { return SceneNode.LocalRotation; }
            set { SceneNode.LocalRotation = value; }
        }

        public float LocalScale
        {
            get { return SceneNode.LocalScale; }
            set { SceneNode.LocalScale = value; }
        }

        public GameObject2D(IDependencyInjector dependencyInjector)
            : base(dependencyInjector)
        {
            SceneNode = Add<SceneNode>();
        }
    }
}