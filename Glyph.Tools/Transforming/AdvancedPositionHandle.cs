using Glyph.Core;
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
        protected override void SetPosition(Vector2 position, IUndoRedoStack undoRedoStack = null)
        {
            IAnchoredPositionController editedObject = EditedObject;
            Vector2 previousPosition = _startPosition;
            
            if (position == previousPosition)
                return;

            undoRedoStack.Execute($"Set position to {position}.",
                () => editedObject.Position = position,
                () => editedObject.Position = previousPosition);
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