using Glyph.Composition;
using Microsoft.Xna.Framework;

namespace Glyph.Core
{
    public class Camera : GlyphContainer, ICamera
    {
        private readonly SceneNode _sceneNode;
        private ReadOnlySceneNode _readOnlySceneNode;
        public float Zoom { get; set; }

        public Vector2 LocalPosition
        {
            get { return _sceneNode.LocalPosition; }
            set { _sceneNode.LocalPosition = value; }
        }

        public float LocalRotation
        {
            get { return _sceneNode.LocalRotation; }
            set { _sceneNode.LocalRotation = value; }
        }

        public Vector2 Position
        {
            get { return _sceneNode.Position; }
        }

        public float Rotation
        {
            get { return _sceneNode.Rotation; }
        }

        public Matrix Matrix
        {
            get
            {
                return Matrix.CreateTranslation((-Position).ToVector3())
                    * Matrix.CreateRotationZ(-Rotation)
                    * Matrix.CreateScale(Zoom);
            }
        }

        public Camera()
        {
            Components.Add(_sceneNode = new SceneNode());
            _readOnlySceneNode = new ReadOnlySceneNode(_sceneNode);
        }

        public override void Initialize()
        {
            _sceneNode.Initialize();
        }
    }
}