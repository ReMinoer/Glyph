using System;
using Diese.Modelization;

namespace Glyph.Composition.Modelization
{
    public interface IGlyphCreator : IGlyphConfigurator, IDisposable
    {
        string Name { get; }
        bool IsInstantiated { get; }
        void Instantiate();
    }

    public interface IGlyphCreator<T> : IGlyphCreator, IGlyphConfigurator<T>, ICreator<T>
        where T : IGlyphComponent
    {
    }
}