using Glyph.Messaging;

namespace Glyph.Composition.Messaging
{
    public class DecompositionMessage<T> : OpenMessage, IDecompositionMessage<T>
        where T : IGlyphComponent
    {
        public T Instance { get; }
        public IGlyphContainer PreviousParent { get; }

        public DecompositionMessage(T instance, IGlyphContainer previousParent)
        {
            Instance = instance;
            PreviousParent = previousParent;
        }
    }
}