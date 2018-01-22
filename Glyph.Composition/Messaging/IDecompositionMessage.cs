using Glyph.Messaging;

namespace Glyph.Composition.Messaging
{
    public interface IDecompositionMessage<out T> : IMessage
        where T : IGlyphComponent
    {
        T Instance { get; }
        IGlyphContainer PreviousParent { get; }
    }
}