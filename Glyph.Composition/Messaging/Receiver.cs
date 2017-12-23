﻿using Glyph.Messaging;

namespace Glyph.Composition.Messaging
{
    [SinglePerParent]
    public class Receiver<TMessage> : GlyphComponent, IEnableable
        where TMessage : IMessage
    {
        private readonly IRouter _router;
        private readonly IInterpreter<TMessage> _interpreter;
        public bool Enabled { get; set; }

        public Receiver(IRouter router, IInterpreter<TMessage> interpreter)
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