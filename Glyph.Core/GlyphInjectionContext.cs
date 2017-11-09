using Diese.Injection;
using Glyph.Core.Injection;

namespace Glyph.Core
{
    public struct GlyphInjectionContext
    {
        public IDependencyRegistry GlobalRegistry { get; }
        public IDependencyRegistry LocalRegistry { get; }
        public LocalDependencyInjector LocalInjectorParent { get; }

        public GlyphInjectionContext([ServiceKey(InjectionScope.Global)]IDependencyRegistry globalRegistry,
                                     [ServiceKey(InjectionScope.Local)]IDependencyRegistry localRegistry,
                                     LocalDependencyInjector localInjectorParent = null)
        {
            GlobalRegistry = globalRegistry;
            LocalRegistry = localRegistry;
            LocalInjectorParent = localInjectorParent;
        }
    }
}