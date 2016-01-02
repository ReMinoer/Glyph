using System.Collections.Generic;
using System.Linq;
using Diese.Composition;

namespace Glyph.Input.Converters
{
    public abstract class InputConverter<TInput, TOutput> : Container<IInputHandler, IInputParent, IInputHandler<TInput>>,
        IInputConverter<TInput, TOutput>
    {
        public bool IsActivated { get; protected set; }
        public TOutput Value { get; protected set; }
        public abstract InputSource InputSource { get; }

        protected InputConverter(string name)
        {
            Name = name;
        }

        public void Update(InputStates inputStates)
        {
            foreach (IInputHandler<TInput> component in this)
                component.Update(inputStates);

            if (this.Any(t => t == null))
            {
                IsActivated = false;
                Value = default(TOutput);
                return;
            }

            HandleInput(this);
        }

        protected abstract void HandleInput(IEnumerable<IInputHandler<TInput>> components);
    }
}