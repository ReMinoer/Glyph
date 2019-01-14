using Glyph.Core;
using Glyph.Graphics.Shapes;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Tools.Base;
using Microsoft.Xna.Framework;

namespace Glyph.Tools
{
    public class PositionHandle : SimpleHandleBase<FilledCircleSprite>
    {
        protected override IArea Area => new Circle(_sceneNode.Position, _spriteSource.Radius);
        
        public Axes Axes { get; set; } = Axes.Both;
        private Vector2 _startPosition;

        public PositionHandle(GlyphInjectionContext context, RootView rootView, ProjectionManager projectionManager)
            : base(context, rootView, projectionManager)
        {
            _spriteSource.Radius = 50;
        }

        protected override void OnGrabbed()
        {
            _startPosition = EditedObject.Position;
        }

        protected override void OnDragging(Vector2 projectedCursorPosition)
        {
            switch (Axes)
            {
                case Axes.Both:
                    EditedObject.Position = projectedCursorPosition;
                    break;
                case Axes.Horizontal:
                    EditedObject.Position = _startPosition.SetX(projectedCursorPosition.X);
                    break;
                case Axes.Vertical:
                    EditedObject.Position = _startPosition.SetY(projectedCursorPosition.Y);
                    break;
            }
        }
    }
}