using System.Collections.Generic;
using System.Linq;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Composition.Injection;
using Glyph.Composition.Tracking;
using Glyph.Core;
using Glyph.Core.Colliders;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Scripting
{
    public class Actor : GlyphContainer, IUpdate, IArea
    {
        private readonly SceneNode _sceneNode;
        private readonly TriggerManager _triggerManager;
        private readonly MessagingTracker<RectangleCollider> _colliderMessagingTracker;
        private readonly List<Trigger> _activatedTriggers;
        private IRectangle _boudingBox;
        private bool _dirtyBoundingBox = true;
        public List<ICollider> Colliders { get; private set; }

        public IRectangle BoundingBox
        {
            get
            {
                if (_dirtyBoundingBox)
                    _boudingBox = MathUtils.GetBoundingBox(Colliders) ?? new CenteredRectangle(_sceneNode.Position, Vector2.Zero);
                return _boudingBox;
            }
        }

        public Actor(SceneNode sceneNode, TriggerManager triggerManager)
        {
            _sceneNode = sceneNode;
            _triggerManager = triggerManager;

            _activatedTriggers = new List<Trigger>();
            Colliders = new List<ICollider>();
        }

        public void Update(ElapsedTime elapsedTime)
        {
            IEnumerable<Trigger> newActivatedTriggers = _triggerManager.Space.GetAllItemsInRange(BoundingBox).Where(trigger => Colliders.Any(collider => collider.Intersects(trigger.Shape))).ToArray();

            IEnumerable<Trigger> enteredTriggers, leavedTriggers;
            if (!_activatedTriggers.SetDiff(newActivatedTriggers, out enteredTriggers, out leavedTriggers))
                return;

            foreach (Trigger trigger in leavedTriggers)
                trigger.OnLeave(this);

            foreach (Trigger trigger in enteredTriggers)
                trigger.OnEnter(this);

            _activatedTriggers.Clear();
            _activatedTriggers.AddRange(newActivatedTriggers);
        }

        public bool ContainsPoint(Vector2 point)
        {
            return Colliders.Any(x => x.ContainsPoint(point));
        }
    }
}