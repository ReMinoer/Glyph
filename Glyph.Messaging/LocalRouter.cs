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

        public override void Register(IInterpreter<TMessage> interpreter)
        {
            _globalRouter.Register(interpreter);
            base.Register(interpreter);
        }

        public override bool Unregister(IInterpreter<TMessage> interpreter)
        {
            if (!base.Unregister(interpreter))
                return false;

            _globalRouter.Unregister(interpreter);

            return true;
        }
    }
}