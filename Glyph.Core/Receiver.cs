using Glyph.Composition;
using Glyph.Messaging;

namespace Glyph.Core
{
    [SinglePerParent]
    public class Receiver<TMessage> : GlyphComponent, IGlyphComponent
        where TMessage : IMessage
    {
        private readonly ITrackingRouter _router;
        private readonly IInterpreter _interpreter;

        public Receiver(ITrackingRouter router, IInterpreter<TMessage> interpreter)
        {
            _router = router;
            _interpreter = interpreter;

            _router.Register<TMessage>(_interpreter);
        }

        public override void Dispose()
        {
            _router.Unregister<TMessage>(_interpreter);
            base.Dispose();
        }
    }
}