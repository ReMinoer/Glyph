using Niddle;
using Glyph.Core.Injection;
using Glyph.Messaging;

namespace Glyph.Application
{
    public class LocalGlyphRegistry : DependencyRegistry
    {
        public LocalGlyphRegistry()
        {
            Link<ITrackingRouter, TrackingRouter>();
            Link<ITrackingRouter, TrackingRouter>(serviceKey: InjectionScope.Local);
            Link<ISubscribableRouter, TrackingRouter>();
            Link<ISubscribableRouter, TrackingRouter>(serviceKey: InjectionScope.Local);
            Link<IRouter, TrackingRouter>();
            Link<IRouter, TrackingRouter>(serviceKey: InjectionScope.Local);
        }
    }
}