namespace Glyph.Composition.Messaging
{
    public interface IInterpreter<in TMessage> : IGlyphComponent
        where TMessage : Message
    {
        void OnMessage(TMessage message);
    }
}
