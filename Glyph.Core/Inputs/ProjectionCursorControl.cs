using System;
using System.Collections.Generic;
using Diese.Collections;
using Fingear;
using Fingear.MonoGame;
using Glyph.Math;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Glyph.Core.Inputs
{
    public class ProjectionCursorControl : CursorControlBase
    {
        private readonly Func<Vector2, IEnumerable<Projection<Vector2>>> _projectionAction;
        public IView InputView { get; }
        public ITransformer Target { get; }

        public ProjectionCursorControl(IView inputView, IView projectionView, ProjectionManager projectionManager)
            : this(null, inputView, projectionView, projectionManager)
        {
        }

        public ProjectionCursorControl(ICursorInput input, IView inputView, IView projectionView, ProjectionManager projectionManager)
            : this(null, input, inputView, projectionView, projectionManager)
        {
        }

        public ProjectionCursorControl(string name, ICursorInput input, IView inputView, IView projectionView, ProjectionManager projectionManager)
            : base(name, input)
        {
            InputView = inputView;
            Target = projectionView;
            _projectionAction = position => projectionManager.ProjectPosition(inputView, position, projectionView);
        }

        public ProjectionCursorControl(IView inputView, ISceneNode projectionSceneNode, ProjectionManager projectionManager)
            : this(null, inputView, projectionSceneNode, projectionManager)
        {
        }

        public ProjectionCursorControl(ICursorInput input, IView inputView, ISceneNode projectionSceneNode, ProjectionManager projectionManager)
            : this(null, input, inputView, projectionSceneNode, projectionManager)
        {
        }
        
        public ProjectionCursorControl(string name, ICursorInput input, IView inputView, ISceneNode projectionSceneNode, ProjectionManager projectionManager)
            : base(name, input)
        {
            InputView = inputView;
            Target = projectionSceneNode;
            _projectionAction = position => projectionManager.ProjectPosition(inputView, position, projectionSceneNode);
        }

        protected override bool ComputeCursor(out System.Numerics.Vector2 value)
        {
            if (!_projectionAction(Input.Value.AsMonoGameVector()).Any(out Projection<Vector2> sceneProjection))
            {
                value = default;
                return false;
            }

            value = sceneProjection.Value.AsSystemVector();
            return true;
        }
    }
}