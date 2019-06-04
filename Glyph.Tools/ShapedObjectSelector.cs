using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Diese.Collections;
using Fingear;
using Fingear.MonoGame;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Core.Tracking;
using Glyph.Math.Shapes;
using Glyph.Messaging;
using Glyph.Space;
using NLog;
using Stave;

namespace Glyph.Tools
{
    public class ShapedObjectSelector : GlyphContainer, IUpdate, IEnableable
    {
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly MessagingSpace<IBoxedComponent> _messagingSpace;
        private readonly InputClientManager _inputClientManager;
        private IBoxedComponent _selection;
        public bool Enabled { get; set; } = true;
        public ReadOnlySpace<IBoxedComponent> Space { get; }
        public IFilter<IInputClient> ClientFilter { get; set; }

        private readonly Controls _interactiveMode;
        private IControl<Vector2> _control;

        public IControl<Vector2> Control
        {
            get => _control;
            set
            {
                if (_control != null)
                    _interactiveMode.Remove(_control);
                
                _control = value;
                
                if (_control != null)
                    _interactiveMode.Add(_control);
            }
        }

        public Predicate<IBoxedComponent> Filter
        {
            get => _messagingSpace.Filter;
            set => _messagingSpace.Filter = value;
        }

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

        public ShapedObjectSelector(InputClientManager inputClientManager, ISubscribableRouter router, IPartitioner partitioner = null)
        {
            _messagingSpace = new MessagingSpace<IBoxedComponent>(router, x => x.Area.BoundingBox, partitioner);
            
            _inputClientManager = inputClientManager;
            _interactiveMode = new Controls(this);
            Components.Add(_interactiveMode);
            
            Space = new ReadOnlySpace<IBoxedComponent>(_messagingSpace);
        }

        public override void Initialize()
        {
            base.Initialize();
            _interactiveMode.Initialize();
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            if (ClientFilter != null && !ClientFilter.Filter(_inputClientManager.InputClient))
                return;
            
            if (Control == null || !Control.IsActive(out Vector2 mousePosition))
                return;

            Logger.Debug($"Pick position: {mousePosition}");

            IEnumerable<IBoxedComponent> inRange = _messagingSpace.GetAllItemsInRange(new CenteredRectangle(mousePosition.AsMonoGameVector(), 1, 1));
            if (Filter != null)
                inRange = inRange.Where(x => Filter(x));

            IBoxedComponent[] array = inRange as IBoxedComponent[] ?? inRange.ToArray();

            if (array.Length != 0)
                Selection = array.MaxBy(x => x, CompareBoxedComponentByRelevance);
            else
                Selection = null;
        }

        private int CompareBoxedComponentByRelevance(IBoxedComponent first, IBoxedComponent second)
        {
            TopLeftRectangle firstBoundingBox = first.Area.BoundingBox;
            TopLeftRectangle secondBoundingBox = second.Area.BoundingBox;

            float firstArea = firstBoundingBox.Height * firstBoundingBox.Width;
            float secondArea = secondBoundingBox.Height * secondBoundingBox.Width;

            int areaComparison = firstArea.CompareTo(secondArea);
            if (areaComparison != 0)
                return -areaComparison;

            if (first.ParentQueue().Contains<IGlyphComponent>(second))
                return 1;
            if (second.ParentQueue().Contains<IGlyphComponent>(first))
                return -1;

            return 0;
        }

        public override void Dispose()
        {
            _messagingSpace.Dispose();
            base.Dispose();
        }
    }
}