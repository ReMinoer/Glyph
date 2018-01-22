using Glyph.Messaging;

namespace Glyph.Composition.Messaging
{
    public class CompositionMessage<T> : OpenMessage, ICompositionMessage<T>
        where T : IGlyphComponent
    {
        public T Instance { get; }
        public IGlyphContainer NewParent { get; }

        public CompositionMessage(T instance, IGlyphContainer newParent)
        {
            Instance = instance;
            NewParent = newParent;
        }
    }
}