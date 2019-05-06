using Niddle;
using Glyph.Core.Injection;
using Niddle.Attributes;

namespace Glyph.Core
{
    public struct GlyphInjectionContext
    {
        public RegistryInjector GlobalInjector { get; }
        public IDependencyRegistry LocalRegistry { get; }
        public LocalDependencyInjector LocalInjectorParent { get; }

        public GlyphInjectionContext([Resolvable(Key = InjectionScope.Global)] RegistryInjector globalInjector,
                                     [Resolvable(Key = InjectionScope.Local)] IDependencyRegistry localRegistry,
                                     LocalDependencyInjector localInjectorParent = null)
        {
            GlobalInjector = globalInjector;
            LocalRegistry = localRegistry;
            LocalInjectorParent = localInjectorParent;
        }
    }
}