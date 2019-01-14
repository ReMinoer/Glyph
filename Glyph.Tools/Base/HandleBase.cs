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
        private Vector2 _grabPosition;

        public IWritableSceneNodeComponent EditedObject { get; set; }
        object IIntegratedEditor.EditedObject => EditedObject;

        public Vector2 LocalPosition
        {
            get => _sceneNode.LocalPosition;
            set => _sceneNode.LocalPosition = value;
        }

        protected abstract IArea Area { get; }

        protected HandleBase(GlyphInjectionContext context, RootView rootView, ProjectionManager projectionManager)
            : base(context)
        {
            _projectionManager = projectionManager;

            _sceneNode = Add<SceneNode>();

            var controls = Add<Controls>();
            controls.Tags.Add(ControlLayerTag.Tools);
            controls.RegisterMany(new IControl[]
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
                        _grabPosition = projectedCursorPosition - _sceneNode.Position;
                        OnGrabbed();
                    }
                }

                if (_grabbed)
                {
                    Projection<Vector2> projection = _projectionManager.ProjectFromPosition(_sceneNode, projectedCursorPosition)
                                                                       .To(EditedObject)
                                                                       .ByRaycast()
                                                                       .First();
                    
                    OnDragging(projection.Value - _grabPosition);
                    _grab.HandleInputs();
                }
            }
            else
            {
                _grabbed = false;
            }
        }
    }
}