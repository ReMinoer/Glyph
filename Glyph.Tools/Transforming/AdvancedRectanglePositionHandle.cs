using Glyph.Core;
using Glyph.Math.Shapes;
using Glyph.Tools.UndoRedo;
using Glyph.UI;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming
{
    public class AdvancedRectanglePositionHandle : AdvancedPositionHandleBase<IAnchoredRectangleController>
    {
        public bool KeyboardEnabled { get; set; }

        public AdvancedRectanglePositionHandle(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context, projectionManager)
        {
            _userInterface.DirectionChanged += OnDirectionChanged;
        }

        protected override Vector2 GetPosition() => EditedObject.Rectangle.Position;
        protected override void SetPosition(Vector2 position, IUndoRedoStack undoRedoStack = null)
        {
            IAnchoredRectangleController editedObject = EditedObject;
            Vector2 previousPosition = _startPosition;

            // Maybe reverse full local transform ?
            if (editedObject.IsLocalRectangle)
                position -= editedObject.Anchor.Position - editedObject.Anchor.LocalPosition;

            if (position == previousPosition)
                return;

            undoRedoStack.Execute($"Set rectangle position to {position}.",
                () => editedObject.Rectangle = new TopLeftRectangle(position, editedObject.Rectangle.Size),
                () => editedObject.Rectangle = new TopLeftRectangle(previousPosition, editedObject.Rectangle.Size));
        }

        private void OnDirectionChanged(object sender, HandlableDirectionEventArgs e)
        {
            if (!Active || !KeyboardEnabled || e.IsHandled)
                return;

            SetPosition(GetPosition() + e.Direction.Normalized().Multiply(10, -10));
            e.Handle();
        }
    }
}