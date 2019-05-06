using System;
using System.Collections.Generic;
using Niddle;
using Niddle.Exceptions;

namespace Glyph.Core.Injection
{
    public class LocalDependencyInjector : RegistryInjector
    {
        public LocalDependencyInjector Parent { get; internal set; }

        public LocalDependencyInjector(IDependencyRegistry localRegistry, LocalDependencyInjector parent = null)
            : base(localRegistry)
        {
            Parent = parent;
        }

        public override object Resolve(Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null, IEnumerable<object> args = null)
        {
            if (TryResolve(out object obj, type, key, origins, dependencyInjector ?? this))
                return obj;

            throw new NotRegisterException(type, key);
        }

        public override bool TryResolve(out object obj, Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null, IEnumerable<object> args = null)
        {
            if (Registry.TryGetFactory(out IDependencyFactory factory, type, key))
            {
                obj = factory.Get(dependencyInjector ?? this);
                return true;
            }

            if (Parent != null && Parent.TryResolve(out obj, type, key, origins, dependencyInjector ?? this))
            {
                return true;
            }

            obj = null;
            return false;
        }
    }
}