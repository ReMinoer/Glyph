using System;
using Glyph.Composition.Messaging;
using Glyph.Scheduling;
using Stave;

namespace Glyph.Composition
{
    // TODO : Add instantiating & disposing router as injectable properties
    public interface IGlyphComponent : IComponent<IGlyphComponent, IGlyphContainer>, IInitializeTask, IRestorable, IDisposable, INotifyDisposed
    {
        bool Enabled { get; set; }
        bool Active { get; }
        bool Visible { get; set; }
        bool Rendered { get; }
        Guid Id { get; }
        string Name { get; set; }
        ComponentRouterSystem Router { get; }
    }
}