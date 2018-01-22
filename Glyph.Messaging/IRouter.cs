namespace Glyph.Messaging
{
    public interface IRouter
    {
        void Send(IMessage message);
    }
}