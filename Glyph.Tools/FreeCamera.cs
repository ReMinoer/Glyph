﻿using System.Collections.Generic;
using System.Linq;
using Diese.Collections;
using Fingear;
using Fingear.Controls;
using Fingear.Converters;
using Fingear.MonoGame;
using Fingear.MonoGame.Inputs;
using Glyph.Core;
using Glyph.Core.Inputs;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Glyph.Tools
{
    public class FreeCamera : GlyphObject
    {
        private readonly InputClientManager _inputClientManager;
        private readonly ProjectionManager _projectionManager;
        private readonly IView _rootView;
        private readonly SceneNode _sceneNode;
        private readonly Camera _camera;
        private IView _view;
        private Vector2 _startCameraRootViewPosition;
        private Vector2 _startCursorRootViewPosition;
        private readonly ProjectionCursorControl _rootViewCursor;
        private readonly ActivityControl _moveCamera;
        private readonly Control<float> _zoomCamera;
        public IDrawClient Client { get; set; }

        public IView View
        {
            get => _view;
            set
            {
                if (_view == value)
                    return;

                if (_view != null)
                    _view.Camera = null;

                _view = value;

                if (_view != null)
                    _view.Camera = _camera;
            }
        }

        public float Zoom
        {
            get => _camera.Zoom;
            set => _camera.Zoom = value;
        }

        public FreeCamera(GlyphInjectionContext context, RootView rootView, InputClientManager inputClientManager, ProjectionManager projectionManager)
            : base(context)
        {
            _rootView = rootView;
            _inputClientManager = inputClientManager;
            _projectionManager = projectionManager;

            _sceneNode = Add<SceneNode>();
            _camera = Add<Camera>();

            var controls = Add<Controls>();
            controls.Tags.Add(ControlLayerTag.Tools);
            controls.RegisterMany(new IControl[]
            {
                _rootViewCursor = new ProjectionCursorControl("Main view cursor", InputSystem.Instance.Mouse.Cursor, _rootView, _rootView, projectionManager),
                _moveCamera = new ActivityControl("Move camera", InputSystem.Instance.Mouse[MouseButton.Right]),
                _zoomCamera = new Control<float>("Zoom camera", InputSystem.Instance.Mouse.Wheel.Force())
            });
            controls.Plan().AtStart();

            Schedulers.Update.Plan(HandleInput);
        }

        private void HandleInput(ElapsedTime elapsedTime)
        {
            if (Client != null && _inputClientManager.DrawClient != Client)
                return;

            if (_zoomCamera.IsActive(out float wheelValue))
            {
                float zoom = _camera.Zoom + wheelValue * 0.1f;
                if (zoom < 0)
                    zoom = 0;

                _camera.Zoom = zoom;
                _zoomCamera.HandleInputs();
            }
             
            if (_moveCamera.IsActive(out InputActivity inputActivity))
            {
                _rootViewCursor.IsActive(out Vector2 cursorRootViewPosition);

                if (inputActivity.IsPressed())
                {
                    if (!inputActivity.IsTriggered())
                    {
                        Vector2 cursorRootViewDelta = cursorRootViewPosition - _startCursorRootViewPosition;
                        Vector2 cameraRootViewPosition = _startCameraRootViewPosition - cursorRootViewDelta;
                        if (_projectionManager.ProjectPosition(_rootView, cameraRootViewPosition, _sceneNode, new ProjectionOptions { Directions = GraphDirections.Successors })
                                              .Where(TransformPathContainsCamera)
                                              .Any(out Projection<Vector2> cameraSceneProjection))
                        {
                            _sceneNode.Position = cameraSceneProjection.Value;
                        }
                    }
                
                    if (_projectionManager.ProjectPosition(_rootView, cursorRootViewPosition, _view, new ProjectionOptions { Directions = GraphDirections.Successors })
                                          .Any(TransformPathContainsCamera))
                    {
                        if (_projectionManager.ProjectPosition(_sceneNode, _rootView, new ProjectionOptions { Directions = GraphDirections.Predecessors })
                                              .Where(TransformPathContainsCamera)
                                              .Any(out Projection<Vector2> cameraRootViewProjection))
                        {
                            _startCursorRootViewPosition = cursorRootViewPosition;
                            _startCameraRootViewPosition = cameraRootViewProjection.Value;
                        }
                    }

                    if (inputActivity.IsTriggered())
                        _moveCamera.HandleInputs();
                }
            }
        }

        private bool TransformPathContainsCamera(Projection<Vector2> projection)
        {
            return projection.TransformerPath.Any(x => x is IView view && view.Camera == _camera);
        }
    }
}