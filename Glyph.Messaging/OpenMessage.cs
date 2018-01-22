namespace Glyph.Messaging
{
    public class OpenMessage : IMessage
    {
        bool IHandlable.IsHandled => false;
    }
}