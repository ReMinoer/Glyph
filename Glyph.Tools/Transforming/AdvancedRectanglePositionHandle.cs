using Glyph.Core;
using Glyph.Math.Shapes;
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
        protected override void SetPosition(Vector2 position) => EditedObject.Rectangle = new TopLeftRectangle(position, EditedObject.Rectangle.Size);

        private void OnDirectionChanged(object sender, HandlableDirectionEventArgs e)
        {
            if (!Active || !KeyboardEnabled || e.IsHandled)
                return;

            SetPosition(GetPosition() + e.Direction.Normalized().Multiply(10, -10));
            e.Handle();
        }
    }
}