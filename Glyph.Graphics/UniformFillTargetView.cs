using System;
using System.Threading.Tasks;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class UniformFillTargetView : GlyphContainer, ILoadContent, IDraw
    {
        private readonly Camera _parentCamera;
        private IView _parentView;
        public TargetView UniformView { get; }

        public bool Visible { get; set; } = true;
        public Predicate<IDrawer> DrawPredicate { get; set; }
        public IFilter<IDrawClient> DrawClientFilter { get; set; }

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

        public UniformFillTargetView(Func<GraphicsDevice> graphicsDeviceFunc)
        {
            Components.Add(_parentCamera = new Camera());
            Components.Add(UniformView = new TargetView(graphicsDeviceFunc));
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

        public override void Initialize()
        {
            base.Initialize();
            _parentCamera.Initialize();
            UniformView.Initialize();
        }

        public async Task LoadContent(IContentLibrary contentLibrary)
        {
            await UniformView.LoadContent(contentLibrary);
        }

        public void Draw(IDrawer drawer)
        {
            if (!this.Displayed(drawer, drawer.Client, this.GetSceneNode()))
                return;

            UniformView.Draw(drawer);
        }
    }
}