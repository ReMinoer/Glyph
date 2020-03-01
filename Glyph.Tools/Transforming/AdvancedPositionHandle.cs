using System.Numerics;
using Glyph.Core;
using Glyph.Tools.Transforming.Base;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Glyph.Tools.Transforming
{
    public class AdvancedRotationHandle : AdvancedHandleBase
    {
        private float _startRotation;

        public AdvancedRotationHandle(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context, projectionManager)
        {
        }

        protected override void OnGrabbed()
        {
            _startRotation = EditedObject.Rotation;
        }
        
        protected override void OnDragging(Vector2 projectedCursorPosition)
        {
            Vector2 pivotPosition = _sceneNode.Position - 2.5f * _sceneNode.LocalPosition;
            float? cursorRotation = (projectedCursorPosition - pivotPosition).ToRotation();
            if (!cursorRotation.HasValue)
                return;
            
            float? rotationFix = _sceneNode.LocalPosition.ToRotation();
            EditedObject.Rotation = _startRotation + cursorRotation.Value - rotationFix.GetValueOrDefault();
        }
    }

    public class AdvancedScaleHandle : AdvancedHandleBase
    {
        private float _startScale;

        public AdvancedScaleHandle(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context, projectionManager)
        {
        }

        protected override void OnGrabbed()
        {
            _startScale = EditedObject.Scale;
        }
        
        protected override void OnDragging(Vector2 projectedCursorPosition)
        {
            Vector2 scaleRatio = (projectedCursorPosition - (_sceneNode.Position - 2 * _sceneNode.LocalPosition)) / _sceneNode.LocalPosition;
            EditedObject.Scale = _startScale * MathHelper.Max(MathHelper.Max(scaleRatio.X, scaleRatio.Y), 0.1f);
        }
    }

    public class AdvancedPositionHandle : AdvancedHandleBase
    {
        private Vector2 _startPosition;
        public Axes Axes { get; set; } = Axes.Both;

        public AdvancedPositionHandle(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context, projectionManager)
        {
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