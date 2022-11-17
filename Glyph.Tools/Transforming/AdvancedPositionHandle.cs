using Glyph.Core;
using Glyph.Math.Shapes;
using Glyph.Tools.UndoRedo;
using Glyph.UI;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming
{
    public class AdvancedPositionHandle : AdvancedPositionHandleBase<IAnchoredPositionController>
    {
        public bool KeyboardEnabled { get; set; }

        public AdvancedPositionHandle(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context, projectionManager)
        {
            _userInterface.DirectionChanged += OnDirectionChanged;
        }

        protected override Vector2 GetPosition() => EditedObject.Position;
        protected override void SetPosition(Vector2 position, bool live, IUndoRedoStack undoRedoStack = null)
        {
            IAnchoredPositionController editedObject = EditedObject;
            Vector2 previousPosition = _startPosition;

            // Maybe reverse full local transform ?
            if (editedObject.IsLocalPosition)
                position -= editedObject.Anchor.Position - editedObject.Anchor.LocalPosition;

            if (position == previousPosition)
                return;

            if (live)
            {
                editedObject.LivePosition = position;
                return;
            }

            undoRedoStack.Execute($"Set position to {position}.",
                () => editedObject.Position = position,
                () => editedObject.Position = previousPosition);
        }

        protected override void ResetPosition()
        {
            EditedObject.Position = _startPosition;
        }

        private void OnDirectionChanged(object sender, HandlableDirectionEventArgs e)
        {
            if (!Active || !KeyboardEnabled || e.IsHandled)
                return;

            SetPosition(GetPosition() + e.Direction.Normalized().Multiply(10, -10), live: false, undoRedoStack: UndoRedoStack);
            e.Handle();
        }
    }
}