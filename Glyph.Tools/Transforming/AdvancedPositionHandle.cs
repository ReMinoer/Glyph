using System;
using Glyph.Core;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Tools.Transforming.Base;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Glyph.Tools.Transforming
{
    public abstract class AdvancedPositionHandle<TController> : AdvancedHandleBase<TController>
        where TController : IAnchoredController
    {
        protected Vector2 _startPosition;
        protected Vector2 _relativeGrabPosition;

        public Axes Axes { get; set; } = Axes.Both;
        public Func<Vector2, Vector2> Revaluation { get; set; }

        public AdvancedPositionHandle(GlyphResolveContext context, ProjectionManager projectionManager)
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

    public class AdvancedPositionHandle : AdvancedPositionHandle<IAnchoredPositionController>
    {
        public AdvancedPositionHandle(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context, projectionManager) {}

        protected override Vector2 GetPosition() => EditedObject.Position;
        protected override void SetPosition(Vector2 position) => EditedObject.Position = position;
    }

    public class AdvancedRectangleBorderPositionHandle : AdvancedPositionHandle<IAnchoredRectangleController>
    {
        private Vector2 _startSize;
        private Vector2 _startCursorProjectedPosition;
        public Axes AxesEditedFromOrigin { get; set; }

        public AdvancedRectangleBorderPositionHandle(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context, projectionManager) { }

        protected override void OnGrabbed(Vector2 cursorPosition)
        {
            base.OnGrabbed(cursorPosition);

            _startSize = EditedObject.Rectangle.Size;
            _startCursorProjectedPosition = ProjectToTargetScene(cursorPosition) - _relativeGrabPosition;
        }

        protected override Vector2 GetPosition()
        {
            return EditedObject.Rectangle.Position;
        }

        protected override void SetPosition(Vector2 position)
        {
            Vector2 clampedPosition = new Vector2(MathHelper.Min(position.X, EditedObject.Rectangle.Right), MathHelper.Min(position.Y, EditedObject.Rectangle.Bottom));
            Vector2 positionDiff = position - _startCursorProjectedPosition;

            Vector2 computedPosition;
            Vector2 computedSize;
            switch (AxesEditedFromOrigin)
            {
                case Axes.Both:
                    computedPosition = clampedPosition;
                    computedSize = _startSize - positionDiff;
                    break;
                case Axes.Horizontal:
                    computedPosition = EditedObject.Rectangle.Position.SetX(clampedPosition.X);
                    computedSize = _startSize + new Vector2(-positionDiff.X, positionDiff.Y);
                    break;
                case Axes.Vertical:
                    computedPosition = EditedObject.Rectangle.Position.SetY(clampedPosition.Y);
                    computedSize = _startSize + new Vector2(positionDiff.X, -positionDiff.Y);
                    break;
                case Axes.None:
                    computedPosition = EditedObject.Rectangle.Position;
                    computedSize = _startSize + positionDiff;
                    break;
                default:
                    throw new NotSupportedException();
            }

            Vector2 correctedComputedSize = new Vector2(MathHelper.Max(computedSize.X, 0), MathHelper.Max(computedSize.Y, 0));

            EditedObject.Rectangle = new TopLeftRectangle(computedPosition, correctedComputedSize);
        }
    }
}