using System.Linq;
using Glyph.Core;
using Glyph.Math;
using Glyph.Tools.Transforming;
using Glyph.UI;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Base
{
    public abstract class HandleBase<TController> : GlyphObject, IIntegratedEditor<TController>
        where TController : IAnchoredController
    {
        private readonly ProjectionManager _projectionManager;
        protected readonly SceneNode _sceneNode;
        protected readonly UserInterface _userInterface;
        
        protected bool Grabbed { get; private set; }

        public IDrawClient RaycastClient { get; set; }

        public TController EditedObject { get; set; }
        object IIntegratedEditor.EditedObject => EditedObject;

        protected abstract IArea Area { get; }

        public Vector2 LocalPosition
        {
            get => _sceneNode.LocalPosition;
            set => _sceneNode.LocalPosition = value;
        }

        public float LocalRotation
        {
            get => _sceneNode.LocalRotation;
            set => _sceneNode.LocalRotation = value;
        }

        protected HandleBase(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context)
        {
            _projectionManager = projectionManager;

            _sceneNode = Add<SceneNode>();

            _userInterface = Add<UserInterface>();
            _userInterface.TouchStarted += OnTouchStarted;
            _userInterface.Touching += OnTouching;
            _userInterface.TouchEnded += OnTouchEnded;
            _userInterface.Cancelled += OnCancelled;
        }

        private void OnTouchStarted(object sender, HandlableTouchEventArgs e)
        {
            Grabbed = false;
            
            if (!Area.ContainsPoint(e.CursorPosition))
                return;
            
            e.Handle();

            Grabbed = true;
            OnGrabbed(e.CursorPosition);
        }
        
        private void OnTouching(object sender, CursorEventArgs e)
        {
            if (!Grabbed)
                return;
            
            Vector2 targetScenePosition = ProjectToTargetScene(e.CursorPosition);
            OnDragging(targetScenePosition);
        }
        
        private void OnTouchEnded(object sender, CursorEventArgs args)
        {
            if (!Grabbed)
                return;

            Grabbed = false;
            OnReleased();
        }

        private void OnCancelled(object sender, HandlableEventArgs e)
        {
            if (!Grabbed)
                return;
            
            Grabbed = false;
            OnCancelled();
        }

        protected abstract void OnGrabbed(Vector2 cursorPosition);
        protected abstract void OnDragging(Vector2 projectedCursorPosition);
        protected abstract void OnReleased();
        protected abstract void OnCancelled();

        protected Vector2 ProjectToTargetScene(Vector2 value)
        {
            return _projectionManager.ProjectFromPosition(_sceneNode, value)
                                     .To(EditedObject.Anchor)
                                     .ByRaycast()
                                     .First().Value;
        }
    }
}