using System;
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
            
            if (targets != GlyphInjectableTargets.NewInstance && serviceKey == null && typeof(IGlyphComponent).IsAssignableFrom(type))
            {
                if (targets.HasFlag(GlyphInjectableTargets.Parent) && type.IsInstanceOfType(_context))
                    return _context;

                if (targets.HasFlag(GlyphInjectableTargets.Fraternal))
                {
                    IGlyphComponent component = _context.GetComponent(type);
                    if (component != null)
                        return component;
                }

                throw new ComponentNotFoundException(type);
            }

            object obj;
            if (_local.TryResolve(out obj, type, injectableAttribute, serviceKey))
                return obj;

            return base.Resolve(type, injectableAttribute, serviceKey);
        }

        public override bool TryResolve(out object obj, Type type, InjectableAttribute injectableAttribute, object serviceKey = null)
        {
            var glyphInjectableAttribute = injectableAttribute as GlyphInjectableAttribute;
            GlyphInjectableTargets targets = glyphInjectableAttribute != null ? glyphInjectableAttribute.Targets : GlyphInjectableTargets.All;

            if (targets != GlyphInjectableTargets.NewInstance && serviceKey == null && typeof(IGlyphComponent).IsAssignableFrom(type))
            {
                if (targets.HasFlag(GlyphInjectableTargets.Parent) && type.IsInstanceOfType(_context))
                {
                    obj = _context;
                    return true;
                }

                if (targets.HasFlag(GlyphInjectableTargets.Fraternal))
                {
                    IGlyphComponent component = _context.GetComponent(type);
                    if (component != null)
                    {
                        obj = component;
                        return true;
                    }
                }

                obj = null;
                return false;
            }

            return _local.TryResolve(out obj, type, injectableAttribute, serviceKey)
                || base.TryResolve(out obj, type, injectableAttribute, serviceKey);
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

            var component = (IGlyphComponent)base.Resolve(type, null, null);
            _context.Add(component);

            return component;
        }
    }
}