using System;
using System.Collections.Generic;
using System.Linq;
using Diese.Collections;
using Fingear;
using Fingear.MonoGame;
using Glyph.Composition;
using Glyph.Composition.Messaging;
using Glyph.Core;
using Glyph.Core.ControlLayers;
using Glyph.Core.Tracking;
using Glyph.Input;
using Glyph.Math.Shapes;
using Glyph.Messaging;
using Glyph.Space;
using NLog;

namespace Glyph.Tools
{
    public class ShapedObjectSelector : GlyphContainer, IUpdate
    {
        static protected readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly MessagingSpace<IBoxedComponent> _messagingSpace;
        private readonly ControlManager _controlManager;
        private IBoxedComponent _selection;
        public ReadOnlySpace<IBoxedComponent> Space { get; }

        public IBoxedComponent Selection
        {
            get => _selection;
            set
            {
                if (_selection == value)
                    return;

                _selection = value;
                SelectionChanged?.Invoke(this, _selection);
            }
        }

        public event EventHandler<IBoxedComponent> SelectionChanged;

        public ShapedObjectSelector(ControlManager controlManager,
            IRouter<InstantiatingMessage<IBoxedComponent>> instantiatingRouter,
            IRouter<DisposingMessage<IBoxedComponent>> disposingRouter,
            IPartitioner partitioner = null)
        {
            _messagingSpace = new MessagingSpace<IBoxedComponent>(instantiatingRouter, disposingRouter, x => x.Area.BoundingBox, partitioner);
            Components.Add(_messagingSpace);
            _controlManager = controlManager;

            Space = new ReadOnlySpace<IBoxedComponent>(_messagingSpace);
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (_controlManager.TryGetLayer(out MouseControls mouseControls) && mouseControls.Left.IsTriggered())
            {
                mouseControls.ScenePosition.IsActive(out System.Numerics.Vector2 mousePosition);

                IEnumerable<IBoxedComponent> inRange = _messagingSpace.GetAllItemsInRange(new CenteredRectangle(mousePosition.AsMonoGameVector(), 1, 1));
                IBoxedComponent[] array = inRange as IBoxedComponent[] ?? inRange.ToArray();

                if (array.Length != 0)
                {
                    Selection = array.MinBy(x => x.Area.BoundingBox.Width * x.Area.BoundingBox.Height);
                    Logger.Trace($"Shaped component selected: {Selection?.Name}");
                }
                else
                {
                    Selection = null;
                    Logger.Trace("Clean selection");
                }
            }
        }
    }
}