using System;
using Glyph.Core;
using Glyph.Math.Shapes;
using Glyph.Tools.UndoRedo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Glyph.Tools.Transforming
{
    public class AdvancedRectangleBorderPositionHandle : AdvancedPositionHandleBase<IAnchoredRectangleController>
    {
        private Vector2 _startSize;
        private Vector2 _startCursorProjectedPosition;
        public Axes AxesEditedFromOrigin { get; set; }

        protected override MouseCursor Cursor
        {
            get
            {
                switch (Axes)
                {
                    case Axes.Both:
                    {
                        switch (AxesEditedFromOrigin)
                        {
                            case Axes.None:
                            case Axes.Both:
                                return MouseCursor.SizeNWSE;
                            case Axes.Horizontal:
                            case Axes.Vertical:
                                return MouseCursor.SizeNESW;
                        }
                        break;
                    }
                    case Axes.Horizontal:
                        return MouseCursor.SizeWE;
                    case Axes.Vertical:
                        return MouseCursor.SizeNS;
                }

                return MouseCursor.SizeAll;
            }
        }

        public AdvancedRectangleBorderPositionHandle(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context, projectionManager)
        {
        }

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

        protected override void SetPosition(Vector2 position, IUndoRedoStack undoRedoStack = null)
        {
            IAnchoredRectangleController editedObject = EditedObject;
            Vector2 previousPosition = _startPosition;

            Vector2 positionDiff = position - _startCursorProjectedPosition;

            // Maybe reverse full local transform ?
            if (editedObject.IsLocalRectangle)
                position -= editedObject.Anchor.Position - editedObject.Anchor.LocalPosition;

            Vector2 clampedPosition = new Vector2(MathHelper.Min(position.X, EditedObject.Rectangle.Right), MathHelper.Min(position.Y, EditedObject.Rectangle.Bottom));

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

            var newRectangle = new TopLeftRectangle(computedPosition, correctedComputedSize);
            var previousRectangle = new TopLeftRectangle(previousPosition, _startSize);

            if (newRectangle.Equals(previousRectangle))
                return;

            undoRedoStack.Execute($"Set rectangle bounds to {newRectangle}.",
                () => editedObject.Rectangle = newRectangle,
                () => editedObject.Rectangle = previousRectangle);
        }
    }
}