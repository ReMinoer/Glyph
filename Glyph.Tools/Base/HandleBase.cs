using System.Linq;
using Glyph.Core;
using Glyph.Math;
using Glyph.UI;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Base
{
    public abstract class HandleBase : GlyphObject, IIntegratedEditor<IWritableSceneNodeComponent>
    {
        private readonly ProjectionManager _projectionManager;
        protected readonly SceneNode _sceneNode;
        
        private Vector2 _relativeGrabPosition;
        private bool _grabbed;

        public IDrawClient RaycastClient { get; set; }

        public IWritableSceneNodeComponent EditedObject { get; set; }
        object IIntegratedEditor.EditedObject => EditedObject;

        protected abstract IArea Area { get; }

        public Vector2 LocalPosition
        {
            get => _sceneNode.LocalPosition;
            set => _sceneNode.LocalPosition = value;
        }

        protected HandleBase(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context)
        {
            _projectionManager = projectionManager;

            _sceneNode = Add<SceneNode>();

            var userInterface = Add<UserInterface>();
            userInterface.TouchStarted += OnTouchStarted;
            userInterface.Touching += OnTouching;
            userInterface.TouchEnded += OnTouchEnded;
        }
        
        private void OnTouchStarted(object sender, HandlableTouchEventArgs e)
        {
            _grabbed = false;
            
            if (!Area.ContainsPoint(e.CursorPosition))
                return;
            
            e.Handle();

            _grabbed = true;
            _relativeGrabPosition = ProjectToTargetScene(e.CursorPosition - _sceneNode.Position + _sceneNode.LocalPosition);
            OnGrabbed();
        }
        
        private void OnTouching(object sender, CursorEventArgs e)
        {
            if (!_grabbed)
                return;
            
            Vector2 targetScenePosition = ProjectToTargetScene(e.CursorPosition);
            OnDragging(targetScenePosition - _relativeGrabPosition);
        }
        
        private void OnTouchEnded(object sender, CursorEventArgs args)
        {
            _grabbed = false;
        }

        protected abstract void OnGrabbed();
        protected abstract void OnDragging(Vector2 projectedCursorPosition);

        private Vector2 ProjectToTargetScene(Vector2 value)
        {
            return _projectionManager.ProjectFromPosition(_sceneNode, value)
                                     .To(EditedObject)
                                     .ByRaycast()
                                     .First().Value;
        }
    }
}