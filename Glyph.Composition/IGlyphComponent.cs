using System;
using System.ComponentModel;
using Stave;

namespace Glyph.Composition
{
    // TODO : Add instantiating & disposing router as injectable properties
    public interface IGlyphComponent : IComponent<IGlyphComponent, IGlyphContainer>, IDisposable, INotifyPropertyChanged
    {
        string Name { get; set; }
        bool Disposed { get; }
        void Initialize();
    }
}