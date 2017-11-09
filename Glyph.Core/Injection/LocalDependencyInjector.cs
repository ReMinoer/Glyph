using System;
using Diese.Injection;
using Diese.Injection.Exceptions;

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

        public override object Resolve(Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null)
        {
            if (TryResolve(out object obj, type, injectableAttribute, serviceKey, instanceOrigins, dependencyInjector ?? this))
                return obj;

            throw new NotRegisterException(type, serviceKey);
        }

        public override bool TryResolve(out object obj, Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null)
        {
            if (Registry.TryGetFactory(out IDependencyFactory factory, type, serviceKey))
            {
                obj = factory.Get(dependencyInjector ?? this);
                return true;
            }

            if (Parent != null && Parent.TryResolve(out obj, type, injectableAttribute, serviceKey, instanceOrigins, dependencyInjector ?? this))
            {
                return true;
            }

            obj = null;
            return false;
        }
    }
}