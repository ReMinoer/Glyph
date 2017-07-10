using System;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Messaging;
using Microsoft.Xna.Framework;

namespace Glyph.Scripting
{
    public class Trigger : GlyphContainer, IEnableable, IShapedComponent<CenteredRectangle>, IMovableShape
    {
        private readonly TriggerManager _triggerManager;
        private readonly IRouter<OnEnterTrigger> _onEnterRouter;
        private readonly IRouter<OnLeaveTrigger> _onLeaveRouter;
        private readonly SceneNode _sceneNode;
        public bool Enabled { get; set; }
        public Vector2 Size { get; set; }
        public bool IsVoid => Shape.IsVoid;

        public Vector2 LocalPosition
        {
            get => _sceneNode.LocalPosition;
            set => _sceneNode.LocalPosition = value;
        }

        Vector2 IMovableShape.Center
        {
            get => Shape.Center;
            set => _sceneNode.Position = value - Size / 2;
        }

        public CenteredRectangle Shape => new CenteredRectangle(_sceneNode.Position, Size);
        IShape IShapedComponent.Shape => Shape;
        TopLeftRectangle IArea.BoundingBox => Shape;
        IArea IBoxedComponent.Area => Shape;
        Vector2 IShape.Center => Shape.Center;

        public event Action<OnEnterTrigger> OnEnter;
        public event Action<OnLeaveTrigger> OnLeave;

        public Trigger(TriggerManager triggerManager, IRouter<OnEnterTrigger> onEnterRouter = null, IRouter<OnLeaveTrigger> onLeaveRouter = null)
        {
            _triggerManager = triggerManager;
            _onEnterRouter = onEnterRouter;
            _onLeaveRouter = onLeaveRouter;

            Components.Add(_sceneNode = new SceneNode());

            _triggerManager.Register(this);
        }

        public override void Initialize()
        {
            base.Initialize();
            _sceneNode.Initialize();
        }

        internal void Enter(Actor actor)
        {
            var message = new OnEnterTrigger(this, actor);
            _onEnterRouter?.Send(message);
            OnEnter?.Invoke(message);
        }

        internal void Leave(Actor actor)
        {
            var message = new OnLeaveTrigger(this, actor);
            _onLeaveRouter?.Send(message);
            OnLeave?.Invoke(message);
        }

        public bool Intersects(TopLeftRectangle rectangle)
        {
            return Shape.Intersects(rectangle);
        }

        public bool Intersects(Circle circle)
        {
            return Shape.Intersects(circle);
        }

        public bool ContainsPoint(Vector2 point)
        {
            return Shape.ContainsPoint(point);
        }

        public override void Dispose()
        {
            _triggerManager.Unregister(this);
            base.Dispose();
        }
    }
}