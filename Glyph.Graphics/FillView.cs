using System;
using Glyph.Core;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class FillView : TargetViewBase
    {
        private Quad _shape;
        private IView _parentView;

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
                Size = ParentView.Size;

                if (_parentView != null)
                    _parentView.SizeChanged += ParentViewOnSizeChanged;

                void ParentViewOnSizeChanged(object sender, Vector2 e) => Size = ParentView.Size;
            }
        }

        public FillView(Func<GraphicsDevice> graphicsDeviceFunc)
            : base(graphicsDeviceFunc)
        {
        }

        protected override void Refresh()
        {
            _shape = _sceneNode.Transformation.Transform(new CenteredRectangle(ParentView.Center, Size));
            base.Refresh();
        }
    }
}