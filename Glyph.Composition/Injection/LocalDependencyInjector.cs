using System;
using Diese.Injection;
using Diese.Injection.Exceptions;

namespace Glyph.Composition.Injection
{
    internal class LocalDependencyInjector : RegistryInjector
    {
        private readonly GlyphCompositeInjector _glyphCompositeInjector;
        public LocalDependencyInjector Parent { get; set; }

        public LocalDependencyInjector(GlyphCompositeInjector glyphCompositeInjector, IDependencyRegistry localRegistry)
            : base(localRegistry)
        {
            _glyphCompositeInjector = glyphCompositeInjector;
        }

        public override object Resolve(Type type, InjectableAttribute injectableAttribute, object serviceKey = null)
        {
            object obj;
            if (TryResolve(out obj, type, injectableAttribute, serviceKey))
                return obj;

            throw new NotRegisterException(type, serviceKey);
        }

        public override bool TryResolve(out object obj, Type type, InjectableAttribute injectableAttribute, object serviceKey = null)
        {
            IDependencyFactory factory;
            if (Registry.TryGetFactory(out factory, type, serviceKey))
            {
                obj = factory.Get(_glyphCompositeInjector);
                return true;
            }

            if (Parent != null && Parent.TryResolve(out obj, type, injectableAttribute, serviceKey))
            {
                return true;
            }

            obj = null;
            return false;
        }
    }
}