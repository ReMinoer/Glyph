using System;
using Glyph.Composition.Messaging;
using Stave;

namespace Glyph.Composition
{
    public interface IInitializeTask
    {
        void Initialize();
    }

    // TODO : Add instantiating & disposing router as injectable properties
    public interface IGlyphComponent : IComponent<IGlyphComponent, IGlyphContainer>, IInitializeTask, IDisposable, INotifyDisposed
    {
        bool Enabled { get; set; }
        bool Active { get; }
        Guid Id { get; }
        string Name { get; set; }
        ComponentRouterSystem Router { get; }
        bool IsDisposed { get; }
    }
}