namespace Glyph.Messaging
{
    public abstract class Message : IHandlable
    {
        public bool IsHandled { get; private set; }

        public void Handle()
        {
            IsHandled = true;
        }
    }
}