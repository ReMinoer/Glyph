using Diese.Injection;
using Glyph.Core.Injection;
using Glyph.Messaging;

namespace Glyph.Core.Messaging
{
    public class LocalRouter : Router
    {
        public IRouter Parent { get; }
        
        public LocalRouter([ServiceKey(InjectionScope.Global)] IRouter parent)
            : base(typeof(ILocalInterpreter<>))
        {
            Parent = parent;
        }

        public override void Send(IMessage message)
        {
            base.Send(message);
            Parent.Send(message);
        }

        public override void Register(IInterpreter item)
        {
            base.Register(item);
            Parent.Register(item);
        }

        public override bool Unregister(IInterpreter item)
        {
            return base.Unregister(item) | Parent.Unregister(item);
        }

        public override void Clear()
        {
            base.Clear();
            Parent.Clear();
        }

        public override void ClearDisposed()
        {
            base.ClearDisposed();
            Parent.ClearDisposed();
        }
    }
}