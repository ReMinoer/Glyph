using System.Collections.Generic;
using System.Linq;
using Diese.Composition;

namespace Glyph.Input.Converters
{
    public abstract class InputConverter<TInput, TOutput> : Synthesizer<IInputHandler, IInputParent, IInputHandler<TInput>>,
        IInputConverter<TInput, TOutput>
    {
        public bool IsActivated { get; protected set; }
        public TOutput Value { get; protected set; }
        public abstract InputSource InputSource { get; }

        protected InputConverter(string name, int size)
            : base(size)
        {
            Name = name;
        }

        protected abstract void HandleInput(IEnumerable<IInputHandler<TInput>> components);

        public void Update(InputStates inputStates)
        {
            foreach (IInputHandler<TInput> component in Components)
                component.Update(inputStates);

            if (Components.Any(t => t == null))
            {
                IsActivated = false;
                Value = default(TOutput);
                return;
            }

            HandleInput(Components);
        }
    }
}