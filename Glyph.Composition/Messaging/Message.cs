namespace Glyph.Composition.Messaging
{
    public abstract class Message
    {
        public bool Cancelled { get; private set; }

        public void Cancel()
        {
            Cancelled = true;
        }
    }
}