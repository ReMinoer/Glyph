using Glyph.Core.Resolvers;
using Niddle;
using Niddle.Attributes;

namespace Glyph.Core
{
    public struct GlyphResolveContext
    {
        public RegistryResolver GlobalResolver { get; }
        public IDependencyRegistry LocalRegistry { get; }
        public LocalDependencyResolver LocalResolverParent { get; }

        public GlyphResolveContext([Resolvable(Key = ResolverScope.Global)] RegistryResolver globalResolver,
                                     [Resolvable(Key = ResolverScope.Local)] IDependencyRegistry localRegistry,
                                     LocalDependencyResolver localResolverParent = null)
        {
            GlobalResolver = globalResolver;
            LocalRegistry = localRegistry;
            LocalResolverParent = localResolverParent;
        }
    }
}