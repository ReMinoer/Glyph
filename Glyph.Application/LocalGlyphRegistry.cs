using Diese.Injection;
using Glyph.Core.Injection;
using Glyph.Core.Messaging;
using Glyph.Messaging;

namespace Glyph.Application
{
    public class LocalGlyphRegistry : DependencyRegistry
    {
        public LocalGlyphRegistry()
        {
            RegisterSingleton<LocalRouter>();
            Link<IRouter, LocalRouter>();
            Link<IRouter, LocalRouter>(serviceKey: InjectionScope.Local);
        }
    }
}