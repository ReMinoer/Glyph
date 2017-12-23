using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Diese.Collections;
using Fingear;
using Fingear.MonoGame;
using Glyph.Composition;
using Glyph.Composition.Messaging;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Core.Tracking;
using Glyph.Math.Shapes;
using Glyph.Messaging;
using Glyph.Space;
using NLog;

namespace Glyph.Tools
{
    public class ShapedObjectSelector : GlyphContainer, IUpdate, IEnableable
    {
        static protected readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly MessagingSpace<IBoxedComponent> _messagingSpace;
        private readonly InputClientManager _inputClientManager;
        private IBoxedComponent _selection;
        public bool Enabled { get; set; } = true;
        public ReadOnlySpace<IBoxedComponent> Space { get; }
        public IFilter<IInputClient> ClientFilter { get; set; }
        public bool HandleInputs { get; set; }

        private readonly Controls _controls;
        private IControl<Vector2> _control;

        public IControl<Vector2> Control
        {
            get => _control;
            set
            {
                if (_control != null)
                    _controls.Unregister(_control);
                
                _control = value;
                
                if (_control != null)
                    _controls.Register(_control);
            }
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

        public ShapedObjectSelector(InputClientManager inputClientManager, ControlManager controlManager, IRouter router, IPartitioner partitioner = null)
        {
            _messagingSpace = new MessagingSpace<IBoxedComponent>(router, x => x.Area.BoundingBox, partitioner);
            Components.Add(_messagingSpace);
            
            _inputClientManager = inputClientManager;
            _controls = new Controls(controlManager);
            _controls.Tags.Add(ControlLayerTag.Tools);
            Components.Add(_controls);
            
            Space = new ReadOnlySpace<IBoxedComponent>(_messagingSpace);
        }

        public override void Initialize()
        {
            base.Initialize();
            _controls.Initialize();
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            if (ClientFilter != null && !ClientFilter.Filter(_inputClientManager.Current))
                return;
            
            if (Control == null || !Control.IsActive(out Vector2 mousePosition))
                return;

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

            if (HandleInputs)
                Control.HandleInputs();
        }
    }
}