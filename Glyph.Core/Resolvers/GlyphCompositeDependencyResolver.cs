using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Resolver;
using Niddle;
using Niddle.Base;

namespace Glyph.Core.Resolvers
{
    public class GlyphCompositeDependencyResolver : DependencyResolverBase
    {
        private readonly GlyphComposite _composite;

        public RegistryResolver Global { get; }
        public LocalDependencyResolver Local { get; }

        internal GlyphObject Parent
        {
            set => Local.Parent = value?.Resolver.Local;
        }

        public GlyphCompositeDependencyResolver(GlyphComposite composite, GlyphResolveContext context)
        {
            _composite = composite;

            Global = context.GlobalResolver;
            Local = new LocalDependencyResolver(context.LocalRegistry, context.LocalResolverParent);
            Local.Registry.Add(GlyphDependency.OnType<LocalDependencyResolver>().Using(Local));

            foreach (Type nestedType in _composite.GetType().GetNestedTypes().Where(x => x.GetConstructors().Length > 0))
                Local.Registry.Add(GlyphDependency.OnType(nestedType));
        }

        public object Resolve(Type type, ResolveTargets targets, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null)
        {
            if (key == null && (origins & InstanceOrigins.Registration) != 0)
            {
                bool browseAllAncestors = (targets & ResolveTargets.BrowseAllAncestors) != 0;

                for (IGlyphContainer parent = _composite; parent != null; parent = parent.Parent)
                {
                    if ((targets & ResolveTargets.Parent) != 0 && type.IsInstanceOfType(parent))
                        return _composite;

                    if ((targets & ResolveTargets.Fraternal) != 0 && TryResolveManyFamily(out IEnumerable objs, parent, type))
                        return objs.First();

                    if (!browseAllAncestors)
                        break;
                }
            }

            if ((targets & ResolveTargets.Local) != 0 && Local.TryResolve(out object obj, type, key, origins, resolver ?? this))
                return obj;

            if (targets == ResolveTargets.Local)
                throw new InvalidOperationException();

            if ((targets & ResolveTargets.Global) != 0)
                return Global.Resolve(type, key, origins, resolver ?? this);

            throw new InvalidOperationException();
        }

        public bool TryResolve(out object obj, Type type, ResolveTargets targets, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null)
        {
            if (key == null && (origins & InstanceOrigins.Registration) != 0)
            {
                bool browseAllAncestors = (targets & ResolveTargets.BrowseAllAncestors) != 0;

                for (IGlyphContainer parent = _composite; parent != null; parent = parent.Parent)
                {
                    if ((targets & ResolveTargets.Parent) != 0 && type.IsInstanceOfType(parent))
                    {
                        obj = _composite;
                        return true;
                    }

                    if ((targets & ResolveTargets.Fraternal) != 0 && TryResolveManyFamily(out IEnumerable objs, parent, type))
                    {
                        obj = objs.FirstOrDefault();
                        return true;
                    }

                    if (!browseAllAncestors)
                        break;
                }
            }
            
            if ((targets & ResolveTargets.Local) != 0 && Local.TryResolve(out obj, type, key, origins, resolver ?? this))
                return true;

            if (targets == ResolveTargets.Local)
            {
                obj = null;
                return false;
            }

            if ((targets & ResolveTargets.Global) != 0)
                return Global.TryResolve(out obj, type, key, origins, resolver ?? this);

            obj = null;
            return false;
        }

        public IEnumerable ResolveMany(Type type, ResolveTargets targets, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null)
        {
            if (targets == 0)
                throw new InvalidOperationException();

            if (key != null)
                yield break;
            
            bool browseAllAncestors = (targets & ResolveTargets.BrowseAllAncestors) != 0;
            bool any = false;

            for (IGlyphContainer parent = _composite; parent != null; parent = parent.Parent)
            {
                foreach (object obj in ResolveManyFamily(parent, type))
                {
                    yield return obj;
                    any = true;
                }

                if (any)
                    yield break;

                if (browseAllAncestors)
                    yield break;
            }
        }

        public bool TryResolveMany(out IEnumerable objs, Type type, ResolveTargets targets, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null)
        {
            if (targets == 0)
                throw new InvalidOperationException();

            if (key != null)
            {
                objs = Enumerable.Empty<object>();
                return false;
            }
            
            bool browseAllAncestors = (targets & ResolveTargets.BrowseAllAncestors) != 0;

            for (IGlyphContainer parent = _composite; parent != null; parent = parent.Parent)
            {
                if (TryResolveManyFamily(out objs, parent, type))
                    return true;

                if (!browseAllAncestors)
                    break;
            }
            
            objs = Enumerable.Empty<object>();
            return false;
        }

        public T Resolve<T>(ResolveTargets targets, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null)
            => (T)Resolve(typeof(T), targets, key, origins, resolver ?? this);

        public IEnumerable<T> ResolveMany<T>(ResolveTargets targets, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null)
            => ResolveMany(typeof(T), targets, key, origins, resolver ?? this).Cast<T>();

        public bool TryResolve<T>(out T obj, ResolveTargets targets, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null)
        {
            if (TryResolve(out object temp, typeof(T), targets, key, origins, resolver ?? this))
            {
                obj = (T)temp;
                return true;
            }

            obj = default(T);
            return false;
        }

        public bool TryResolveMany<T>(out IEnumerable<T> objs, ResolveTargets targets, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null)
        {
            if (TryResolveMany(out IEnumerable enumerable, typeof(T), targets, key, origins, resolver ?? this))
            {
                objs = enumerable.Cast<T>();
                return true;
            }

            objs = null;
            return false;
        }

        public override object Resolve(Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null)
            => Resolve(type, GetTargets(type, args), key, origins, resolver);

        public override bool TryResolve(out object obj, Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null)
            => TryResolve(out obj, type, GetTargets(type, args), key, origins, resolver);

        public override IEnumerable ResolveMany(Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null)
            => ResolveMany(type, GetTargets(type, args), key, origins, resolver);

        public override bool TryResolveMany(out IEnumerable objs, Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null)
            => TryResolveMany(out objs, type, GetTargets(type, args), key, origins, resolver);

        static private ResolveTargets GetTargets(Type type, IEnumerable<object> args)
        {
            if (args != null && args.AnyOfType(out ResolveTargetsAttribute attribute))
                return attribute.Targets;
            
            return ResolveTargets.All;
        }

        private IEnumerable ResolveManyFamily(IGlyphComponent parent, Type type)
        {
            bool any = false;

            foreach (IGlyphComponent component in parent.Components.OfType(type))
            {
                yield return component;

                any = true;
            }

            if (!any)
                throw new InvalidOperationException();
        }

        private bool TryResolveManyFamily(out IEnumerable objs, IGlyphComponent parent, Type type)
        {
            objs = Enumerable.Empty<object>();
            bool any = false;

            foreach (IGlyphComponent component in parent.Components.OfType(type))
            {
                objs = objs.Concat(component);
                any = true;
            }

            return any;
        }

        internal T Add<T>()
            where T : IGlyphComponent
        {
            var component = this.WithInstance(Local).Resolve<T>(origins: InstanceOrigins.Instantiation);
            _composite.Add(component);

            return component;
        }

        internal T Add<T>(Action<T> beforeAdding)
            where T : IGlyphComponent
        {
            var component = this.WithInstance(Local).Resolve<T>(origins: InstanceOrigins.Instantiation);
            beforeAdding(component);
            _composite.Add(component);

            return component;
        }

        internal object Add(Type type)
        {
            if (!typeof(IGlyphComponent).IsAssignableFrom(type))
                throw new InvalidCastException($"Type must implements {typeof(IGlyphComponent)} !");

            var component = (IGlyphComponent)this.WithInstance(Local).Resolve(type, origins: InstanceOrigins.Instantiation);
            _composite.Add(component);

            return component;
        }
    }
}