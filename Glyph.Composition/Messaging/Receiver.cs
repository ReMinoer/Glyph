namespace Glyph.Composition.Messaging
{
    public class Receiver<TMessage> : GlyphComponent, IEnableable
        where TMessage : Message
    {
        private readonly IRouter<TMessage> _router;
        private readonly IInterpreter<TMessage> _interpreter;
        public bool Enabled { get; set; }

        public Receiver(IRouter<TMessage> router, IInterpreter<TMessage> interpreter)
        {
            _router = router;
            _interpreter = interpreter;

            _router.Register(_interpreter);
        }

        public override void Dispose()
        {
            _router.Unregister(_interpreter);
            base.Dispose();
        }
    }
}