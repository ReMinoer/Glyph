using System;

namespace Glyph.Composition.Layers
{
    public abstract class LayerBase<TLayer> : ILayer<TLayer>
        where TLayer : class, ILayer<TLayer>
    {
        public ILayerRoot<TLayer> Root { get; private set; }
        public virtual int Index { get; protected set; }

        protected LayerBase(ILayerRoot<TLayer> root)
        {
            Root = root;
        }

        public int CompareTo(ILayer other)
        {
            return Index.CompareTo(other.Index);
        }

        public abstract void Initialize();
        public abstract void Update(ElapsedTime elapsedTime);
    }
}