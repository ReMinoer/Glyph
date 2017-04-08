using System;
using System.Linq;
using Fingear;
using Fingear.MonoGame;
using Glyph.Composition;
using Glyph.Composition.Messaging;
using Glyph.Core;
using Glyph.Core.Tracking;
using Glyph.Input;
using Glyph.Input.StandardControls;
using Glyph.Math.Shapes;
using Glyph.Messaging;
using Glyph.Space;

namespace Glyph.Tools
{
    public class ShapedObjectSelector : GlyphContainer, IUpdate
    {
        private readonly MessagingSpace<IShapedComponent> _messagingSpace;
        private readonly ControlManager _controlManager;
        private IShapedComponent _selection;

        public IShapedComponent Selection
        {
            get => _selection;
            set
            {
                _selection = value;
                SelectionChanged?.Invoke(this, _selection);
            }
        }

        public event EventHandler<IShapedComponent> SelectionChanged;

        public ShapedObjectSelector(ControlManager controlManager,
            IRouter<InstantiatingMessage<IShapedComponent>> instantiatingRouter,
            IRouter<DisposingMessage<IShapedComponent>> disposingRouter,
            IPartitioner partitioner = null)
        {
            Components.Add(_messagingSpace = new MessagingSpace<IShapedComponent>(instantiatingRouter, disposingRouter, x => x.Area.BoundingBox, partitioner));
            _controlManager = controlManager;
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (_controlManager.TryGetLayer(out MouseControls mouseControls)
                && mouseControls.Left.IsTriggered()
                && mouseControls.ScenePosition.IsActive(out System.Numerics.Vector2 mousePosition))
                Selection = _messagingSpace.GetAllItemsInRange(new CenteredRectangle(mousePosition.AsMonoGameVector(), 0, 0)).FirstOrDefault();
        }
    }
}