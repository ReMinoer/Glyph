using System;
using System.Linq;
using Glyph.Core;
using Glyph.Math;
using Glyph.Tools.Transforming;
using Glyph.UI;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Base
{
    public interface IHandle : IIntegratedEditor
    {
        IDrawClient RaycastClient { get; set; }
        event EventHandler Grabbed;
        event EventHandler Dragging;
        event EventHandler Released;
        event EventHandler Cancelled;
    }

    public interface IHandle<TController> : IHandle, IIntegratedEditor<TController>
    {
        new TController EditedObject { get; set; }
    }

    public abstract class HandleBase<TController> : GlyphObject, IHandle<TController>
        where TController : IAnchoredController
    {
        private readonly ProjectionManager _projectionManager;
        protected readonly SceneNode _sceneNode;
        protected readonly UserInterface _userInterface;
        
        protected bool IsGrabbed { get; private set; }

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

        public event EventHandler Grabbed;
        public event EventHandler Dragging;
        public event EventHandler Released;
        public event EventHandler Cancelled;

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
            IsGrabbed = false;
            
            if (!Active || !Area.ContainsPoint(e.CursorPosition))
                return;
            
            e.Handle();

            IsGrabbed = true;
            OnGrabbed(e.CursorPosition);
            Grabbed?.Invoke(this, EventArgs.Empty);
        }
        
        private void OnTouching(object sender, CursorEventArgs e)
        {
            if (!IsGrabbed)
                return;
            
            Vector2 targetScenePosition = ProjectToTargetScene(e.CursorPosition);
            OnDragging(targetScenePosition);
            Dragging?.Invoke(this, EventArgs.Empty);
        }
        
        private void OnTouchEnded(object sender, CursorEventArgs args)
        {
            if (!IsGrabbed)
                return;

            IsGrabbed = false;
            OnReleased();
            Released?.Invoke(this, EventArgs.Empty);
        }

        private void OnCancelled(object sender, HandlableEventArgs e)
        {
            if (!IsGrabbed)
                return;

            IsGrabbed = false;
            OnCancelled();
            Cancelled?.Invoke(this, EventArgs.Empty);
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