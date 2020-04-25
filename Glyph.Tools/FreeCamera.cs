using System.Linq;
using Diese.Collections;
using Fingear.Controls;
using Fingear.Inputs;
using Fingear.Inputs.Converters;
using Fingear.MonoGame;
using Fingear.MonoGame.Inputs;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
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

        public FreeCamera(GlyphResolveContext context, RootView rootView, InputClientManager inputClientManager, ProjectionManager projectionManager)
            : base(context)
        {
            _rootView = rootView;
            _inputClientManager = inputClientManager;
            _projectionManager = projectionManager;

            _sceneNode = Add<SceneNode>();
            _camera = Add<Camera>();

            var controls = Add<Controls>();
            controls.AddMany(new IControl[]
            {
                _rootViewCursor = new ProjectionCursorControl("Main view cursor", InputSystem.Instance.Mouse.Cursor, _rootView, _rootView, projectionManager),
                _moveCamera = new ActivityControl("Move camera", InputSystem.Instance.Mouse[MouseButton.Right]),
                _zoomCamera = new Control<float>("Zoom camera", InputSystem.Instance.Mouse.Wheel.Force())
            });

            Schedulers.Update.Plan(HandleInput);
        }

        public void ShowTarget(IBoxedComponent boxedComponent)
        {
            TopLeftRectangle boundingBox = boxedComponent.Area.BoundingBox;
            const float marginScale = 1.1f;

            float widthZoom = View.DisplayedRectangle.Width / (boundingBox.Width * marginScale) * _camera.Zoom;
            float heightZoom = View.DisplayedRectangle.Height / (boundingBox.Height * marginScale) * _camera.Zoom;
            float wantedZoom = MathHelper.Min(widthZoom, heightZoom);

            _sceneNode.Position = boundingBox.Center;
            _camera.Zoom = wantedZoom;
        }

        private void HandleInput(ElapsedTime elapsedTime)
        {
            if (Client != null && _inputClientManager.DrawClient != Client)
                return;

            if (_zoomCamera.IsActive(out float wheelValue))
            {
                float zoom = _camera.Zoom + wheelValue * (_camera.Zoom / 2);
                if (zoom < 0)
                    zoom = 0;

                _camera.Zoom = zoom;
            }
             
            if (_moveCamera.IsActive(out InputActivity inputActivity))
            {
                _rootViewCursor.IsActive(out Vector2 cursorRootViewPosition);

                if (inputActivity.IsPressed())
                {
                    if (!_projectionManager.ProjectFromPosition(_rootView, cursorRootViewPosition).To(_view)
                        .InDirections(GraphDirections.Successors)
                        .Any(TransformPathContainsCamera))
                        return;

                    if (!_projectionManager.ProjectFromPosition(_sceneNode).To(_rootView)
                        .InDirections(GraphDirections.Predecessors)
                        .Where(TransformPathContainsCamera)
                        .Any(out Projection<Vector2> cameraRootViewProjection))
                        return;

                    if (!inputActivity.IsTriggered())
                    {
                        Vector2 cursorRootViewDelta = cursorRootViewPosition - _startCursorRootViewPosition;
                        Vector2 newCameraRootViewPosition = cameraRootViewProjection.Value - cursorRootViewDelta;

                        if (_projectionManager.ProjectFromPosition(_rootView, newCameraRootViewPosition).To(_sceneNode)
                            .InDirections(GraphDirections.Successors)
                            .Where(TransformPathContainsCamera)
                            .Any(out Projection<Vector2> cameraSceneProjection))
                        {
                            _sceneNode.Position = cameraSceneProjection.Value;
                        }
                    }

                    _startCursorRootViewPosition = cursorRootViewPosition;
                }
            }
        }

        private bool TransformPathContainsCamera(Projection<Vector2> projection)
        {
            return projection.TransformerPath.Any(x => x is IView view && view.Camera == _camera);
        }
    }
}