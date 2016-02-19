using System;
using System.Collections.Generic;
using System.Reflection;
using Diese.Composition;

namespace Glyph.Composition
{
    // TODO : Add instantiating & disposing router as injectable properties
    public interface IGlyphComponent : IComponent<IGlyphComponent, IGlyphParent>, IDisposable
    {
        IEnumerable<PropertyInfo> InjectableProperties { get; }
        void Initialize();
    }
}