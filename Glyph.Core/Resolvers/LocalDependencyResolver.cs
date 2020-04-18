using System;
using System.Collections.Generic;
using Niddle;
using Niddle.Dependencies;
using Niddle.Exceptions;

namespace Glyph.Core.Resolvers
{
    public class LocalDependencyResolver : RegistryResolver
    {
        public LocalDependencyResolver Parent { get; internal set; }

        public LocalDependencyResolver(IDependencyRegistry localRegistry, LocalDependencyResolver parent = null)
            : base(localRegistry)
        {
            Parent = parent;
        }

        public override object Resolve(Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null)
        {
            if (TryResolve(out object obj, type, key, origins, resolver ?? this))
                return obj;

            throw new NotRegisterException(type, key);
        }

        public override bool TryResolve(out object obj, Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null)
        {
            if (Registry.TryGetFactory(out IDependencyFactory factory, type, key))
            {
                obj = factory.Get(resolver ?? this);
                return true;
            }

            if (Parent != null && Parent.TryResolve(out obj, type, key, origins, resolver ?? this))
            {
                return true;
            }

            obj = null;
            return false;
        }
    }
}