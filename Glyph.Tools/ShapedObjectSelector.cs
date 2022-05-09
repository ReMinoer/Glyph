using System;
using System.Collections.Generic;
using System.Linq;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Core.Tracking;
using Glyph.Math.Shapes;
using Glyph.Resolver;
using Glyph.Space;
using Glyph.UI;
using Niddle.Attributes;
using Stave;

namespace Glyph.Tools
{
    public class ShapedObjectSelector : GlyphContainer
    {
        private readonly MessagingSpace<IBoxedComponent> _messagingSpace;
        private readonly InputClientManager _inputClientManager;
        private UserInterface _userInterface;
        
        public ReadOnlySpace<IBoxedComponent> Space { get; }
        public IBoxedComponent Selection { get; set; }
        public IFilter<IInputClient> ClientFilter { get; set; }
        
        public Predicate<IBoxedComponent> Filter
        {
            get => _messagingSpace.Filter;
            set => _messagingSpace.Filter = value;
        }
        
        [Resolvable, ResolveTargets(ResolveTargets.Fraternal | ResolveTargets.BrowseAllAncestors)]
        public UserInterface UserInterface
        {
            get => _userInterface;
            set
            {
                if (_userInterface != null)
                    _userInterface.TouchStarted -= OnTouchStarted;

                _userInterface = value;

                if (_userInterface != null)
                    _userInterface.TouchStarted += OnTouchStarted;
            }
        }

        public event EventHandler<IBoxedComponent> SelectionChanged;

        public ShapedObjectSelector(InputClientManager inputClientManager, IGlyphComponent root, IPartitioner partitioner = null)
        {
            _inputClientManager = inputClientManager;

            _messagingSpace = new MessagingSpace<IBoxedComponent>(root, x => x.Area.BoundingBox, partitioner);
            Space = new ReadOnlySpace<IBoxedComponent>(_messagingSpace);
        }

        private void OnTouchStarted(object sender, HandlableTouchEventArgs e)
        {
            if (!Enabled)
                return;

            if (ClientFilter != null && !ClientFilter.Filter(_inputClientManager.InputClient))
                return;

            e.Handle();

            IEnumerable<IBoxedComponent> inRange = _messagingSpace.GetAllItemsInRange(new CenteredRectangle(e.CursorPosition, 1, 1));
            if (Filter != null)
                inRange = inRange.Where(x => Filter(x));

            IBoxedComponent[] array = inRange as IBoxedComponent[] ?? inRange.ToArray();

            if (array.Length != 0)
                SetSelection(array.MaxBy(x => x, CompareBoxedComponentByRelevance));
            else
                SetSelection(null);
        }

        private void SetSelection(IBoxedComponent selection)
        {
            if (Selection == selection)
                return;

            Selection = selection;
            SelectionChanged?.Invoke(this, selection);
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

            if (first.AllParents().Contains<IGlyphComponent>(second))
                return 1;
            if (second.AllParents().Contains<IGlyphComponent>(first))
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