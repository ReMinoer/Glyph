using System;
using Diese.Injection;
using Diese.Injection.Exceptions;

namespace Glyph.Core.Injection
{
    internal class LocalDependencyInjector : RegistryInjector
    {
        public LocalDependencyInjector Parent { get; set; }

        public LocalDependencyInjector(IDependencyRegistry localRegistry)
            : base(localRegistry)
        {
        }

        public override object Resolve(Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            object obj;
            if (TryResolve(out obj, type, injectableAttribute, serviceKey, dependencyInjector ?? this))
                return obj;

            throw new NotRegisterException(type, serviceKey);
        }

        public override bool TryResolve(out object obj, Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            IDependencyFactory factory;
            if (Registry.TryGetFactory(out factory, type, serviceKey))
            {
                obj = factory.Get(dependencyInjector ?? this);
                return true;
            }

            if (Parent != null && Parent.TryResolve(out obj, type, injectableAttribute, serviceKey, dependencyInjector ?? this))
            {
                return true;
            }

            obj = null;
            return false;
        }
    }
}