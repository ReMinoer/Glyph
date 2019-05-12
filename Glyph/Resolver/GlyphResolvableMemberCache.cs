﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Diese.Collections;
using Niddle;
using Niddle.Attributes.Base;

namespace Glyph.Resolver
{
    public struct GlyphResolvableInjectable
    {
        public IResolvableInjectable<object, object, object> ResolvableInjectable;
        public ResolveTargets Targets;
    }

    public class GlyphResolvableMemberCache : IResolvableMembersProvider<object>
    {
        private readonly Dictionary<Type, GlyphResolvableInjectable[]> _cache;

        public GlyphResolvableMemberCache()
        {
            _cache = new Dictionary<Type, GlyphResolvableInjectable[]>();
        }

        public IEnumerable<IResolvableInjectable<object, object, object>> ForType<T>() => ForType(typeof(T));
        public IEnumerable<IResolvableInjectable<object, object, object>> ForType(Type type) => ForType(type, ResolveTargets.All).Select(x => x.ResolvableInjectable);
        public IEnumerable<GlyphResolvableInjectable> ForType<T>(ResolveTargets targets) => ForType(typeof(T), targets);
        public IEnumerable<GlyphResolvableInjectable> ForType(Type type, ResolveTargets targets)
        {
            if (!_cache.TryGetValue(type, out GlyphResolvableInjectable[] resolvableInjectables))
                _cache[type] = resolvableInjectables = ForTypeAndTargetInternal(type).ToArray();

            return resolvableInjectables.Where(x => (x.Targets & targets) != 0);
        }

        public IEnumerable<GlyphResolvableInjectable> ForTypeAndTargetInternal(Type type)
        {
            foreach (FieldInfo fieldInfo in type.GetRuntimeFields())
            {
                if (!fieldInfo.IsPublic)
                    continue;

                if (IsResolvableMember(fieldInfo, out ResolveTargets memberTargets))
                    yield return new GlyphResolvableInjectable
                    {
                        ResolvableInjectable = fieldInfo.AsResolvableInjectable<object>(),
                        Targets = memberTargets
                    };
            }

            foreach (PropertyInfo propertyInfo in type.GetRuntimeProperties())
            {
                if ((propertyInfo.SetMethod == null || !propertyInfo.SetMethod.IsPublic)
                    && (propertyInfo.GetMethod == null || !propertyInfo.GetMethod.IsPublic))
                    continue;

                if (IsResolvableMember(propertyInfo, out ResolveTargets memberTargets))
                    yield return new GlyphResolvableInjectable
                    {
                        ResolvableInjectable = propertyInfo.AsResolvableInjectable<object>(),
                        Targets = memberTargets
                    };
            }
        }

        static private bool IsResolvableMember(MemberInfo memberInfo, out ResolveTargets targets)
        {
            Attribute[] attributes = memberInfo.GetCustomAttributes().ToArray();
            
            targets = ResolveTargets.All;
            if (!attributes.AnyOfType<ResolvableAttributeBase>())
                return false;

            if (attributes.AnyOfType(out ResolveTargetsAttribute targetsAttribute))
                targets = targetsAttribute.Targets;

            return true;
        }
    }
}