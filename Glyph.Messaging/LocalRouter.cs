namespace Glyph.Messaging
{
    public class LocalRouter<TMessage> : RouterBase<TMessage>
        where TMessage : Message
    {
        private readonly GlobalRouter<TMessage> _globalRouter;

        public LocalRouter(GlobalRouter<TMessage> globalRouter)
        {
            _globalRouter = globalRouter;
        }

        public override void Send(TMessage message)
        {
            foreach (IInterpreter<TMessage> interpreter in Interpreters)
            {
                if (message.Cancelled)
                    return;

                interpreter.Interpret(message);
            }
        }

        public override void Register(IInterpreter<TMessage> interpreter)
        {
            _globalRouter.Register(interpreter);
            base.Register(interpreter);
        }

        public override void Unregister(IInterpreter<TMessage> interpreter)
        {
            base.Unregister(interpreter);
            _globalRouter.Unregister(interpreter);
        }
    }
}