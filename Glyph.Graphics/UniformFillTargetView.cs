using Glyph.Core;
using Microsoft.Xna.Framework;

namespace Glyph.Graphics
{
    public class UniformFillTargetView : GlyphObject
    {
        private readonly Camera _parentCamera;
        private IView _parentView;
        public TargetView UniformView { get; }

        public IView ParentView
        {
            get => _parentView;
            set
            {
                if (_parentView == value)
                    return;

                if (_parentView != null)
                    _parentView.SizeChanged -= ParentViewOnSizeChanged;

                _parentView = value;
                _parentView.Camera = _parentCamera;
                RefreshParentCamera();

                if (_parentView != null)
                    _parentView.SizeChanged += ParentViewOnSizeChanged;
            }
        }

        public ICamera Camera
        {
            get => UniformView.Camera;
            set => UniformView.Camera = value;
        }

        public Vector2 Size
        {
            get => UniformView.Size;
            set
            {
                UniformView.Size = value;
                RefreshParentCamera();
            }
        }

        public UniformFillTargetView(GlyphResolveContext context)
            : base(context)
        {
            _parentCamera = Add<Camera>();
            UniformView = Add<TargetView>();
        }

        private void ParentViewOnSizeChanged(object sender, Vector2 e)
        {
            RefreshParentCamera();
        }

        private void RefreshParentCamera()
        {
            if (ParentView != null)
                _parentCamera.Zoom = UniformView.Size.X / UniformView.Size.Y > ParentView.Size.X / ParentView.Size.Y
                    ? ParentView.Size.X / UniformView.Size.X
                    : ParentView.Size.Y / UniformView.Size.Y;
            else
                _parentCamera.Zoom = 1;
        }
    }
}