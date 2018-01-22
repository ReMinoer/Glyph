using System;

namespace Glyph
{
    public class Handlable : IHandlable
    {
        public bool IsHandled { get; private set; }
        public void Handle() => IsHandled = true;
    }

    public class Handlable<T> : IHandlable
    {
        public bool IsHandled => Handler != null;
        public T Handler { get; private set; }

        public void HandleBy(T handler)
        {
            if (Handler != null)
                throw new InvalidOperationException();

            Handler = handler;
        }
    }
}