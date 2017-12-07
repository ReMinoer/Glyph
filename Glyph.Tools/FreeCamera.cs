using Diese.Collections;
using Fingear;
using Fingear.Controls;
using Fingear.Converters;
using Fingear.MonoGame;
using Fingear.MonoGame.Inputs;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Graphics;
using Glyph.Math.Shapes;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Glyph.Tools
{
    public class FreeCamera : GlyphObject
    {
        private readonly InputClientManager _inputClientManager;
        private readonly SceneNode _sceneNode;
        private readonly Camera _camera;
        private View _view;
        private TopLeftRectangle _viewBoundingBox;
        private Vector2 _startCameraVirtualPosition;
        private Vector2 _startCameraScenePosition;
        private System.Numerics.Vector2 _startMouseVirtualPosition;
        private bool _moving;
        private readonly ReferentialCursorControl _virtualScreenCursor;
        private readonly ActivityControl _moveCamera;
        private readonly Control<float> _zoomCamera;
        public IDrawClient Client { get; set; }

        public View View
        {
            get => _view;
            set
            {
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

        public FreeCamera(GlyphInjectionContext context, InputClientManager inputClientManager)
            : base(context)
        {
            _inputClientManager = inputClientManager;

            _sceneNode = Add<SceneNode>();
            _camera = Add<Camera>();

            var controls = Add<Controls>();
            controls.Tags.Add(ControlLayerTag.Tools);
            controls.RegisterMany(new IControl[]
            {
                _virtualScreenCursor = _inputClientManager.CursorControls.VirtualScreenPosition,
                _moveCamera = new ActivityControl("Move camera", InputSystem.Instance.Mouse[MouseButton.Right]),
                _zoomCamera = new Control<float>("Zoom camera", InputSystem.Instance.Mouse.Wheel.Force())
            });
            controls.Plan().AtStart();

            Schedulers.Update.Plan(HandleInput);
        }

        private void HandleInput(ElapsedTime elapsedTime)
        {
            if (Client != null && _inputClientManager.Current != Client)
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
                _virtualScreenCursor.IsActive(out System.Numerics.Vector2 virtualPosition);
                IView view = ViewManager.Main.GetViewAtPoint(virtualPosition.AsMonoGameVector(), Client);
                if (view == _view)
                {
                    if (inputActivity.IsTriggered())
                    {
                        _moving = true;
                        _startMouseVirtualPosition = virtualPosition;
                        _startCameraVirtualPosition = ViewManager.Main.GetPositionOnVirtualScreen(_view.SceneToView(_sceneNode.Position), _view);
                        _startCameraScenePosition = _camera.Position;
                        _viewBoundingBox = view.BoundingBox;
                    }
                    else if (inputActivity.IsPressed() && _moving)
                    {
                        System.Numerics.Vector2 delta = virtualPosition - _startMouseVirtualPosition;
                        ViewManager.Main.GetViewAtPoint(_startCameraVirtualPosition + delta.AsMonoGameVector(), Client, out Vector2 viewPosition);
                        _sceneNode.Position = -((viewPosition - _viewBoundingBox.Size / 2) / _camera.Zoom) + _startCameraScenePosition;
                    }
                }

                if (inputActivity.IsTriggered())
                    _moveCamera.HandleInputs();
            }
            else
                _moving = false;
        }
    }
}