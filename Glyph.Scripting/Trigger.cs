using Glyph.Composition;
using Glyph.Core;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Messaging;
using Microsoft.Xna.Framework;

namespace Glyph.Scripting
{
    public class Trigger : GlyphContainer, IEnableable, IShapedComponent<IRectangle>, IShape
    {
        private readonly TriggerManager _triggerManager;
        private readonly IRouter<OnEnterTrigger> _onEnterRouter;
        private readonly IRouter<OnLeaveTrigger> _onLeaveRouter;
        private readonly SceneNode _sceneNode;
        public bool Enabled { get; set; }
        public Vector2 Size { get; set; }

        public Vector2 LocalPosition
        {
            get { return _sceneNode.LocalPosition; }
            set { _sceneNode.LocalPosition = value; }
        }

        public IRectangle Shape
        {
            get { return new CenteredRectangle(_sceneNode.Position, Size); }
        }

        ISceneNode IBoxedComponent.SceneNode
        {
            get { return new ReadOnlySceneNode(_sceneNode); }
        }

        IShape IShapedComponent.Shape
        {
            get { return Shape; }
        }

        IRectangle IArea.BoundingBox
        {
            get { return Shape; }
        }

        IArea IBoxedComponent.Area
        {
            get { return Shape; }
        }

        Vector2 IShape.Center
        {
            get { return Shape.Center; }
            set { _sceneNode.Position = value - Size / 2; }
        }

        public Trigger(TriggerManager triggerManager, IRouter<OnEnterTrigger> onEnterRouter, IRouter<OnLeaveTrigger> onLeaveRouter)
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

        internal void OnEnter(Actor actor)
        {
            _onEnterRouter.Send(new OnEnterTrigger(this, actor));
        }

        internal void OnLeave(Actor actor)
        {
            _onLeaveRouter.Send(new OnLeaveTrigger(this, actor));
        }

        public bool Intersects(IRectangle rectangle)
        {
            return Shape.Intersects(rectangle);
        }

        public bool Intersects(ICircle circle)
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