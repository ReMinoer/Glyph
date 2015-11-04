using System;

namespace Glyph.Composition.Layers
{
    public interface ILayer : IComparable<ILayer>
    {
        int Index { get; }
        void Initialize();
        void Update(ElapsedTime elapsedTime);
    }

    public interface ILayer<TLayer> : ILayer, IComparable<TLayer>
        where TLayer : class, ILayer<TLayer>
    {
        ILayerRoot<TLayer> Root { get; }
    }
}