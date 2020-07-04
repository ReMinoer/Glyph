using Diese.Collections;
using Glyph.Composition;
using Niddle.Attributes;

namespace Glyph.Core
{
    public class Flipper : GlyphComponent, IFlipableComponent
    {
        private Axes _axes;

        public Axes Axes
        {
            get => _axes;
            set
            {
                if (_axes == value)
                    return;

                Flip(_axes ^ value);
            } 
        }

        [Populatable(PopulateMethodName = nameof(ITracker<object>.Register))]
        public ITracker<IFlipableComponent> Flipables { get; } = new DisposableTracker<IFlipableComponent>();

        public void Flip(Axes axes)
        {
            _axes ^= axes;

            foreach (IFlipableComponent flipable in Flipables)
                flipable.Flip(axes);
        }
    }
}