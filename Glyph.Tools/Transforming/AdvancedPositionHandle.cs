﻿using Glyph.Core;
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
        protected override void SetPosition(Vector2 position) => EditedObject.Position = position;

        private void OnDirectionChanged(object sender, HandlableDirectionEventArgs e)
        {
            if (!Active || !KeyboardEnabled || e.IsHandled)
                return;

            SetPosition(GetPosition() + e.Direction.Normalized().Multiply(10, -10));
            e.Handle();
        }
    }
}