namespace Glyph.Messaging
{
    public abstract class Message : IMessage
    {
        public bool IsHandled { get; private set; }
        public void Handle() => IsHandled = true;
    }
}