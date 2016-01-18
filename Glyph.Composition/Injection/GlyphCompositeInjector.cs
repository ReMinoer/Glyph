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

        public override object Resolve(Type type, object serviceKey = null)
        {
            if (serviceKey == null && typeof(IGlyphComponent).IsAssignableFrom(type))
            {
                if (type.IsInstanceOfType(_context))
                    return _context;

                IGlyphComponent component = _context.GetComponent(type);
                if (component == null)
                    throw new ComponentNotFoundException(type);

                return component;
            }

            object obj;
            if (_local.TryResolve(out obj, type, serviceKey))
                return obj;

            return base.Resolve(type, serviceKey);
        }

        public override bool TryResolve(out object obj, Type type, object serviceKey = null)
        {
            if (serviceKey == null && typeof(IGlyphComponent).IsAssignableFrom(type))
            {
                if (type.IsInstanceOfType(_context))
                {
                    obj = _context;
                    return true;
                }

                IGlyphComponent component = _context.GetComponent(type);
                if (component != null)
                {
                    obj = component;
                    return true;
                }
            }

            return _local.TryResolve(out obj, type, serviceKey)
                || base.TryResolve(out obj, type, serviceKey);
        }

        internal T Add<T>()
        {
            return (T)Add(typeof(T));
        }

        internal object Add(Type type)
        {
            if (!typeof(IGlyphComponent).IsAssignableFrom(type))
                throw new InvalidCastException(string.Format("Type must implements {0} !", typeof(IGlyphComponent)));

            var component = (IGlyphComponent)base.Resolve(type);
            _context.Add(component);

            return component;
        }
    }
}