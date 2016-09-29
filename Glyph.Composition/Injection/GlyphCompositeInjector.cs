using System;
using System.Linq;
using Diese.Collections;
using Diese.Injection;
using Glyph.Composition.Exceptions;

namespace Glyph.Composition.Injection
{
    public class GlyphCompositeInjector : RegistryInjector
    {
        private readonly GlyphComposite _context;
        private readonly LocalDependencyInjector _local;

        internal GlyphSchedulableBase Parent
        {
            set { _local.Parent = value.Injector._local; }
        }

        public IDependencyRegistry LocalRegistry
        {
            get { return _local.Registry; }
        }

        public IDependencyInjector Base
        {
            get { return Resolve<IDependencyInjector>(); }
        }

        public GlyphCompositeInjector(GlyphComposite context, IDependencyRegistry globalRegistry, IDependencyRegistry localRegistry)
            : base(globalRegistry)
        {
            _context = context;
            _local = new LocalDependencyInjector(this, localRegistry);
        }

        public override object Resolve(Type type, InjectableAttribute injectableAttribute, object serviceKey = null)
        {
            var glyphInjectableAttribute = injectableAttribute as GlyphInjectableAttribute;
            GlyphInjectableTargets targets = glyphInjectableAttribute != null ? glyphInjectableAttribute.Targets : GlyphInjectableTargets.All;

            if (targets == 0)
                throw new InvalidOperationException();

            if (targets.HasFlag(GlyphInjectableTargets.Parent) || targets.HasFlag(GlyphInjectableTargets.Fraternal))
            {
                if (targets.HasFlag(GlyphInjectableTargets.Parent) && type.IsInstanceOfType(_context))
                    return _context;

                if (serviceKey == null && typeof(IGlyphComponent).IsAssignableFrom(type))
                {
                    if (targets.HasFlag(GlyphInjectableTargets.Fraternal))
                    {
                        IGlyphComponent component;
                        if (_context.OfType(type).Any(out component))
                            return component;
                    }

                    throw new ComponentNotFoundException(type);
                }
            }

            object obj;
            if (targets.HasFlag(GlyphInjectableTargets.Local) && _local.TryResolve(out obj, type, injectableAttribute, serviceKey))
                return obj;
            if (targets == GlyphInjectableTargets.Local)
                throw new ComponentNotFoundException(type);

            if (targets.HasFlag(GlyphInjectableTargets.Global))
                return base.Resolve(type, injectableAttribute, serviceKey);

            throw new InvalidOperationException();
        }

        public override bool TryResolve(out object obj, Type type, InjectableAttribute injectableAttribute, object serviceKey = null)
        {
            var glyphInjectableAttribute = injectableAttribute as GlyphInjectableAttribute;
            GlyphInjectableTargets targets = glyphInjectableAttribute != null ? glyphInjectableAttribute.Targets : GlyphInjectableTargets.All;

            if (targets == 0)
                throw new InvalidOperationException();

            if (targets.HasFlag(GlyphInjectableTargets.Parent) || targets.HasFlag(GlyphInjectableTargets.Fraternal))
                if (serviceKey == null && typeof(IGlyphComponent).IsAssignableFrom(type))
                {
                    if (targets.HasFlag(GlyphInjectableTargets.Parent) && type.IsInstanceOfType(_context))
                    {
                        obj = _context;
                        return true;
                    }

                    if (targets.HasFlag(GlyphInjectableTargets.Fraternal))
                    {
                        IGlyphComponent component;
                        if (_context.OfType(type).Any(out component))
                        {
                            obj = component;
                            return true;
                        }
                    }

                    obj = null;
                    return false;
                }

            if (targets.HasFlag(GlyphInjectableTargets.Local) && _local.TryResolve(out obj, type, injectableAttribute, serviceKey))
                return true;
            if (targets == GlyphInjectableTargets.Local)
            {
                obj = null;
                return false;
            }

            if (targets.HasFlag(GlyphInjectableTargets.Global))
                return base.TryResolve(out obj, type, injectableAttribute, serviceKey);

            obj = null;
            return false;
        }

        public T ResolveLocal<T>(object serviceKey = null)
        {
            return Resolve<T>(new GlyphInjectableAttribute(GlyphInjectableTargets.Local), serviceKey);
        }

        public object ResolveLocal(Type type, object serviceKey = null)
        {
            return Resolve(type, new GlyphInjectableAttribute(GlyphInjectableTargets.Local), serviceKey);
        }

        internal T Add<T>()
            where T : IGlyphComponent
        {
            return (T)Add(typeof(T));
        }

        internal object Add(Type type)
        {
            if (!typeof(IGlyphComponent).IsAssignableFrom(type))
                throw new InvalidCastException(string.Format("Type must implements {0} !", typeof(IGlyphComponent)));

            var component = (IGlyphComponent)base.Resolve(type, null);
            _context.Add(component);

            return component;
        }
    }
}