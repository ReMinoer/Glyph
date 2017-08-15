using System;
using System.Collections;
using System.Linq;
using Diese.Collections;
using Diese.Injection;
using Glyph.Composition;
using Glyph.Composition.Exceptions;
using Glyph.Injection;

namespace Glyph.Core.Injection
{
    public class GlyphCompositeInjector : RegistryInjector
    {
        private readonly GlyphComposite _context;
        private readonly LocalDependencyInjector _local;

        internal GlyphObject Parent
        {
            set => _local.Parent = value.Injector._local;
        }

        public IDependencyRegistry LocalRegistry => _local.Registry;
        public IDependencyInjector Base => Resolve<IDependencyInjector>();

        public GlyphCompositeInjector(GlyphComposite context, IDependencyRegistry globalRegistry, IDependencyRegistry localRegistry)
            : base(globalRegistry)
        {
            _context = context;
            _local = new LocalDependencyInjector(localRegistry);
        }

        public override object Resolve(Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            GlyphInjectableTargets targets = GetTargets(type, injectableAttribute);

            if (serviceKey == null)
            {
                if ((targets & GlyphInjectableTargets.Parent) != 0 && type.IsInstanceOfType(_context))
                    return _context;
                if ((targets & GlyphInjectableTargets.Fraternal) != 0)
                    return ResolveManyFamily(type).First<object>();
            }

            if ((targets & GlyphInjectableTargets.Local) != 0 && _local.TryResolve(out object obj, type, injectableAttribute, serviceKey, dependencyInjector ?? this))
                return obj;
            if (targets == GlyphInjectableTargets.Local)
                throw new ComponentNotFoundException(type);

            if ((targets & GlyphInjectableTargets.Global) != 0)
                return base.Resolve(type, injectableAttribute, serviceKey, dependencyInjector ?? this);

            throw new InvalidOperationException();
        }

        public override bool TryResolve(out object obj, Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            GlyphInjectableTargets targets = GetTargets(type, injectableAttribute);

            if (serviceKey == null)
            {
                if ((targets & GlyphInjectableTargets.Parent) != 0 && type.IsInstanceOfType(_context))
                {
                    obj = _context;
                    return true;
                }
                if ((targets & GlyphInjectableTargets.Fraternal) != 0 && TryResolveManyFamily(out IEnumerable objs, type))
                {
                    obj = objs.FirstOrDefault();
                    return true;
                }
            }

            if ((targets & GlyphInjectableTargets.Local) != 0 && _local.TryResolve(out obj, type, injectableAttribute, serviceKey, dependencyInjector ?? this))
                return true;
            if (targets == GlyphInjectableTargets.Local)
            {
                obj = null;
                return false;
            }

            if ((targets & GlyphInjectableTargets.Global) != 0)
                return base.TryResolve(out obj, type, injectableAttribute, serviceKey, dependencyInjector ?? this);

            obj = null;
            return false;
        }

        public override IEnumerable ResolveMany(Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            var glyphInjectableAttribute = injectableAttribute as IGlyphInjectableAttribute;
            if (glyphInjectableAttribute == null)
                return base.ResolveMany(type, injectableAttribute, serviceKey, dependencyInjector ?? this);

            GlyphInjectableTargets targets = glyphInjectableAttribute.Targets;
            if (targets == 0)
                throw new InvalidOperationException();

            if (serviceKey != null)
                return Enumerable.Empty<object>();

            return ResolveManyFamily(type);
        }

        public override bool TryResolveMany(out IEnumerable objs, Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            var glyphInjectableAttribute = injectableAttribute as IGlyphInjectableAttribute;
            if (glyphInjectableAttribute == null)
                return base.TryResolveMany(out objs, type, injectableAttribute, serviceKey, dependencyInjector ?? this);

            GlyphInjectableTargets targets = glyphInjectableAttribute.Targets;
            if (targets == 0)
                throw new InvalidOperationException();

            if (serviceKey != null)
            {
                objs = Enumerable.Empty<object>();
                return false;
            }

            return TryResolveManyFamily(out objs, type);
        }

        static private GlyphInjectableTargets GetTargets(Type type, InjectableAttributeBase injectableAttribute)
        {
            GlyphInjectableTargets targets;
            if (injectableAttribute is IGlyphInjectableAttribute glyphInjectableAttribute)
            {
                targets = glyphInjectableAttribute.Targets;
                if (targets == 0)
                    throw new InvalidOperationException();
            }
            else
            {
                targets = GlyphInjectableTargets.Global | GlyphInjectableTargets.Local | GlyphInjectableTargets.Parent;
                if (typeof(IGlyphComponent).IsAssignableFrom(type))
                    targets |= GlyphInjectableTargets.Fraternal;
            }

            return targets;
        }

        private IEnumerable ResolveManyFamily(Type type)
        {
            bool any = false;
            
            foreach (IGlyphComponent component in _context.Components.OfType(type))
            {
                yield return component;
                any = true;
            }

            if (!any)
                throw new InvalidOperationException();
        }

        private bool TryResolveManyFamily(out IEnumerable objs, Type type)
        {
            objs = Enumerable.Empty<object>();
            bool any = false;
            
            foreach (IGlyphComponent component in _context.Components.OfType(type))
            {
                objs = objs.Concat(component);
                any = true;
            }

            return any;
        }

        public T ResolveLocal<T>(object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            return Resolve<T>(new GlyphInjectableAttribute(GlyphInjectableTargets.Local), serviceKey, dependencyInjector);
        }

        public object ResolveLocal(Type type, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            return Resolve(type, new GlyphInjectableAttribute(GlyphInjectableTargets.Local), serviceKey, dependencyInjector);
        }

        internal T Add<T>()
            where T : IGlyphComponent
        {
            return (T)Add(typeof(T));
        }

        internal object Add(Type type)
        {
            if (!typeof(IGlyphComponent).IsAssignableFrom(type))
                throw new InvalidCastException($"Type must implements {typeof(IGlyphComponent)} !");

            var component = (IGlyphComponent)base.Resolve(type);
            _context.Add(component);

            return component;
        }
    }
}