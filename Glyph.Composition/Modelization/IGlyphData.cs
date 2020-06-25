﻿using System;
using System.Collections.Generic;
using Diese.Collections.Observables.ReadOnly;
using Simulacra;
using Simulacra.Injection;

namespace Glyph.Composition.Modelization
{
    public interface IGlyphData : IBindableData, IDependencyResolverClient
    {
        string DisplayName { get; }
        new IGlyphComponent BindedObject { get; }
        IReadOnlyObservableCollection<IGlyphData> Children { get; }
        IEnumerable<Type> SerializationKnownTypes { get; set; }
    }
}