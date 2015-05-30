using System.Collections.Generic;
using Diese.Composition;

namespace Glyph.Input.Converters
{
    public abstract class InputConverter<TInput, TOutput> : Synthesizer<IInputHandler,IInputHandler<TInput>>, IInputConverter<TInput, TOutput>
    {
        public bool IsActivated { get; protected set; }
        public TOutput Value { get; protected set; }
        public abstract InputSource InputSource { get; }

        protected InputConverter(string name, int size)
            : base(size)
        {
            Name = name;
        }

        public void Update(InputStates inputStates)
        {
            foreach (IInputHandler<TInput> component in Components)
                component.Update(inputStates);

            for (int i = 0; i < Components.Length; i++)
                if (Components[i] == null)
                {
                    IsActivated = false;
                    Value = default(TOutput);
                    return;
                }

            HandleInput(Components);
        }

        protected abstract void HandleInput(IEnumerable<IInputHandler<TInput>> components);
    }
}