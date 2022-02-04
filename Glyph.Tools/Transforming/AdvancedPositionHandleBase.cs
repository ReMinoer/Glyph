using System;
using Glyph.Core;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Tools.Transforming.Base;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Glyph.Tools.Transforming
{
    public abstract class AdvancedPositionHandleBase<TController> : AdvancedHandleBase<TController>
        where TController : IAnchoredController
    {
        protected Vector2 _startPosition;
        protected Vector2 _relativeGrabPosition;

        public Axes Axes { get; set; } = Axes.Both;
        public Func<Vector2, Vector2> Revaluation { get; set; }

        public AdvancedPositionHandleBase(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context, projectionManager)
        {
        }

        protected override void OnGrabbed(Vector2 cursorPosition)
        {
            base.OnGrabbed(cursorPosition);

            _startPosition = GetPosition();
            _relativeGrabPosition = ProjectToTargetScene(cursorPosition - _sceneNode.Position + _sceneNode.LocalPosition);
        }
        
        protected override void OnDragging(Vector2 projectedCursorPosition)
        {
            projectedCursorPosition -= _relativeGrabPosition;

            if (Revaluation != null)
                projectedCursorPosition = Revaluation(projectedCursorPosition);

            switch (Axes)
            {
                case Axes.Both:
                    SetPosition(projectedCursorPosition);
                    break;
                case Axes.Horizontal:
                    SetPosition(GetClosestPointOnAxis(projectedCursorPosition, Vector2.UnitX));
                    break;
                case Axes.Vertical:
                    SetPosition(GetClosestPointOnAxis(projectedCursorPosition, Vector2.UnitY));
                    break;
            }
        }

        private Vector2 GetClosestPointOnAxis(Vector2 position, Vector2 axis)
        {
            return MathUtils.GetClosestToPointOnLine(position, new Segment(_sceneNode.Position, _sceneNode.Transform(axis)));
        }

        protected override void OnCancelled()
        {
            base.OnCancelled();
            SetPosition(_startPosition);
        }

        protected abstract Vector2 GetPosition();
        protected abstract void SetPosition(Vector2 position);
    }
}