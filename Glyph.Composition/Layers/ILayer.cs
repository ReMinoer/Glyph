using System;

namespace Glyph.Composition.Layers
{
    public interface ILayer : IComparable<ILayer>
    {
        int Index { get; set; }
        void Initialize();
        void Update(ElapsedTime elapsedTime);
    }

    public interface ILayer<out TLayer> : ILayer
        where TLayer : class, ILayer<TLayer>
    {
        ILayerRoot<TLayer> Root { get; }
    }
}