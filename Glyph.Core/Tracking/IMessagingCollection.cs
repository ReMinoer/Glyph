﻿using System;
using System.Collections.Generic;
using Glyph.Composition.Messaging;
using Glyph.Messaging;

namespace Glyph.Core.Tracking
{
    public interface IMessagingCollection<T> : IEnumerable<T>, IInterpreter<InstantiatingMessage<T>>, IInterpreter<DisposingMessage<T>>
    {
        IReadOnlyCollection<T> NewInstances { get; }
        Predicate<T> Filter { get; }
        event Action<T> Registered;
        event Action<T> Unregistered;
        void CleanNewInstances();
    }
}