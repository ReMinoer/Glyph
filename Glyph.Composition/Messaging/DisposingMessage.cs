namespace Glyph.Messaging
{
    public class DisposingMessage<T> : Message
    {
        public T Instance { get; private set; }

        public DisposingMessage(T instance)
        {
            Instance = instance;
        }
    }
}