using Glyph.Messaging;

namespace Glyph.Composition.Messaging
{
    public interface ICompositionMessage<out T> : IMessage
        where T : IGlyphComponent
    {
        T Instance { get; }
        IGlyphContainer NewParent { get; }
    }
}