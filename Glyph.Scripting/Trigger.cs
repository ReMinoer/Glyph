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
        private readonly IRouter _router;
        private readonly SceneNode _sceneNode;
        public bool Enabled { get; set; }
        public Vector2 Size { get; set; }
        public Predicate<Actor> ActorPredicate { get; set; }

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
        public bool IsVoid => Shape.IsVoid;

        public event Action<OnEnterTrigger> OnEnter;
        public event Action<OnLeaveTrigger> OnLeave;

        public Trigger(TriggerManager triggerManager, IRouter router = null)
        {
            _triggerManager = triggerManager;
            _router = router;

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
            _router?.Send(message);
            OnEnter?.Invoke(message);
        }

        internal void Leave(Actor actor)
        {
            var message = new OnLeaveTrigger(this, actor);
            _router?.Send(message);
            OnLeave?.Invoke(message);
        }

        private bool Interpret(TriggerMessageBase message)
        {
            return !message.IsHandled && (ActorPredicate == null || ActorPredicate(message.Actor));
        }

        public bool ContainsPoint(Vector2 point) => Shape.ContainsPoint(point);
        public bool Intersects(Segment segment) => Shape.Intersects(segment);
        public bool Intersects<T>(T edgedShape) where T : IEdgedShape => Shape.Intersects(edgedShape);
        public bool Intersects(Circle circle) => Shape.Intersects(circle);

        public override void Dispose()
        {
            _triggerManager.Unregister(this);
            base.Dispose();
        }
    }
}