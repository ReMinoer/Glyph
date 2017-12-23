using System;
using System.Collections.Generic;

namespace Glyph.Reflection
{
    public class GlyphTypeInfoProvider
    {
        private Dictionary<Type, GlyphTypeInfo> TypeInfoDictionary { get; } = new Dictionary<Type, GlyphTypeInfo>();
        
        public GlyphTypeInfo this[Type type]
        {
            get
            {
                if (!TypeInfoDictionary.TryGetValue(type, out GlyphTypeInfo typeInfo))
                    TypeInfoDictionary.Add(type, typeInfo = new GlyphTypeInfo(type));
                return typeInfo;
            }
        }
    }
}