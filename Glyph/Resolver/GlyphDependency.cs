using System;
using Niddle;
using Niddle.Dependencies.Builders;

namespace Glyph.Resolver
{
    static public class GlyphDependency
    {
        static public GlyphResolvableMemberCache ResolvableMembersCache { get; } = new GlyphResolvableMemberCache();

        static public ITypeDependencyBuilder<object> OnType(Type type) => Dependency.OnType(type).ResolvingMembersFrom(ResolvableMembersCache);
        static public ITypeDependencyBuilder<T> OnType<T>() => Dependency.OnType<T>().ResolvingMembersFrom(ResolvableMembersCache);
        static public IGenericDependencyBuilder OnGeneric(Type typeDefinition) => Dependency.OnGeneric(typeDefinition).ResolvingMembersFrom(ResolvableMembersCache);
    }
}