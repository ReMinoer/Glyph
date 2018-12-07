using System;

namespace Glyph.Core.Layers
{
    public interface ILayer : IComparable<ILayer>
    {
        ILayerRoot Root { get; }
        int Index { get; set; }
        void Initialize();
        void Update(ElapsedTime elapsedTime);
    }
}