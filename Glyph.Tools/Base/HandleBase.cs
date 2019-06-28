using System.Linq;
using Diese.Collections;
using Fingear;
using Fingear.Controls;
using Fingear.MonoGame;
using Fingear.MonoGame.Inputs;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Base
{
    public abstract class HandleBase : GlyphObject, IIntegratedEditor<IWritableSceneNodeComponent>
    {
        protected readonly ProjectionManager _projectionManager;
        protected readonly SceneNode _sceneNode;
        
        private readonly ProjectionCursorControl _projectedCursor;
        private readonly ActivityControl _grab;

        private bool _grabbed;
        private Vector2 _relativeGrabPosition;

        public IWritableSceneNodeComponent EditedObject { get; set; }
        object IIntegratedEditor.EditedObject => EditedObject;

        public Vector2 LocalPosition
        {
            get => _sceneNode.LocalPosition;
            set => _sceneNode.LocalPosition = value;
        }

        protected abstract IArea Area { get; }

        public IDrawClient RaycastClient
        {
            get => _projectedCursor.RaycastClient;
            set => _projectedCursor.RaycastClient = value;
        }

        protected HandleBase(GlyphResolveContext context, RootView rootView, ProjectionManager projectionManager)
            : base(context)
        {
            _projectionManager = projectionManager;

            _sceneNode = Add<SceneNode>();

            var interactiveMode = Add<Controls>();
            interactiveMode.AddMany(new IControl[]
            {
                _projectedCursor = new ProjectionCursorControl("Virtual cursor", InputSystem.Instance.Mouse.Cursor, rootView, _sceneNode, projectionManager),
                _grab = new ActivityControl("Grab handle", InputSystem.Instance.Mouse[MouseButton.Left])
            });
            
            Schedulers.Update.Plan(HandleInput).AtEnd();
        }

        protected abstract void OnGrabbed();
        protected abstract void OnDragging(Vector2 projectedCursorPosition);

        private void HandleInput(ElapsedTime elapsedTime)
        {
            if (_grab.IsActive(out InputActivity grabActivity) && (_grabbed || grabActivity.IsTriggered()))
            {
                _projectedCursor.IsActive(out Vector2 projectedCursorPosition);

                if (grabActivity.IsTriggered())
                {
                    if (Area.ContainsPoint(projectedCursorPosition))
                    {
                        _grabbed = true;
                        _relativeGrabPosition = ProjectToTargetScene(projectedCursorPosition - _sceneNode.Position + _sceneNode.LocalPosition);
                        OnGrabbed();
                    }
                }

                if (_grabbed)
                {
                    Vector2 targetScenePosition = ProjectToTargetScene(projectedCursorPosition);
                    OnDragging(targetScenePosition - _relativeGrabPosition);
                }
            }
            else
            {
                _grabbed = false;
            }
        }

        private Vector2 ProjectToTargetScene(Vector2 value)
        {
            return _projectionManager.ProjectFromPosition(_sceneNode, value)
                                     .To(EditedObject)
                                     .ByRaycast()
                                     .First().Value;
        }
    }
}