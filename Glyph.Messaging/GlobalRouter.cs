namespace Glyph.Messaging
{
    public class GlobalRouter<TMessage> : RouterBase<TMessage>
        where TMessage : Message
    {
        public GlobalRouter()
        {
        } 
    }
}