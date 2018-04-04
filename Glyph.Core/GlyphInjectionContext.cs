using Niddle;
using Glyph.Core.Injection;

namespace Glyph.Core
{
    public struct GlyphInjectionContext
    {
        public RegistryInjector GlobalInjector { get; }
        public IDependencyRegistry LocalRegistry { get; }
        public LocalDependencyInjector LocalInjectorParent { get; }

        public GlyphInjectionContext([ServiceKey(InjectionScope.Global)] RegistryInjector globalInjector,
                                     [ServiceKey(InjectionScope.Local)] IDependencyRegistry localRegistry,
                                     LocalDependencyInjector localInjectorParent = null)
        {
            GlobalInjector = globalInjector;
            LocalRegistry = localRegistry;
            LocalInjectorParent = localInjectorParent;
        }
    }
}