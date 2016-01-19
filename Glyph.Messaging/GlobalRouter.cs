namespace Glyph.Messaging
{
    public class GlobalRouter<TMessage> : RouterBase<TMessage>
        where TMessage : Message
    {
        public override void Send(TMessage message)
        {
            foreach (IInterpreter<TMessage> interpreter in Interpreters)
                interpreter.OnMessage(message);
        }
    }
}