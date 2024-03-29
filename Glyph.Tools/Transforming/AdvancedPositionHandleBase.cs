﻿using System;
using Glyph.Core;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Tools.Transforming.Base;
using Glyph.Tools.UndoRedo;
using Microsoft.Xna.Framework.Input;
using Niddle.Attributes;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Glyph.Tools.Transforming
{
    public abstract class AdvancedPositionHandleBase<TController> : AdvancedHandleBase<TController>
        where TController : IAnchoredController
    {
        protected Vector2 _startPosition;
        protected Vector2 _relativeGrabPosition;
        private Vector2 _lastProjectedCursorPosition;

        public Axes Axes { get; set; } = Axes.Both;
        public Func<Vector2, Vector2> Revaluation { get; set; }

        [Resolvable]
        public IUndoRedoStack UndoRedoStack { get; set; }

        protected override MouseCursor Cursor
        {
            get
            {
                switch (Axes)
                {
                    case Axes.Horizontal:
                        return MouseCursor.SizeWE;
                    case Axes.Vertical:
                        return MouseCursor.SizeNS;
                    default:
                        return MouseCursor.SizeAll;
                }
            }
        }

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

            switch (Axes)
            {
                case Axes.Horizontal:
                    projectedCursorPosition = GetClosestPointOnAxis(projectedCursorPosition, Vector2.UnitX);
                    break;
                case Axes.Vertical:
                    projectedCursorPosition = GetClosestPointOnAxis(projectedCursorPosition, Vector2.UnitY);
                    break;
            }

            if (Revaluation != null)
                projectedCursorPosition = Revaluation(projectedCursorPosition);

            SetPosition(projectedCursorPosition, live: true);
            _lastProjectedCursorPosition = projectedCursorPosition;
        }

        private Vector2 GetClosestPointOnAxis(Vector2 position, Vector2 axis)
        {
            return MathUtils.GetClosestToPointOnLine(position, new Segment(_sceneNode.Position, _sceneNode.Transform(axis)));
        }

        protected override void OnReleased()
        {
            base.OnReleased();
            SetPosition(_lastProjectedCursorPosition, live: false, UndoRedoStack);
        }

        protected override void OnCancelled()
        {
            base.OnCancelled();
            ResetPosition();
        }

        protected abstract Vector2 GetPosition();
        protected abstract void SetPosition(Vector2 position, bool live, IUndoRedoStack undoRedoStack = null);
        protected abstract void ResetPosition();
    }
}