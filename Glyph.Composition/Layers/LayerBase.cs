namespace Glyph.Composition.Layers
{
    public abstract class LayerBase<TLayer> : ILayer<TLayer>
        where TLayer : class, ILayer<TLayer>
    {
        public ILayerRoot<TLayer> Root { get; private set; }
        public virtual int Index { get; set; }

        protected LayerBase(ILayerRoot<TLayer> root)
        {
            Root = root;
        }

        public int CompareTo(ILayer other)
        {
            return Index.CompareTo(other.Index);
        }

        public virtual void Initialize()
        {
        }

        public virtual void Update(ElapsedTime elapsedTime)
        {
        }
    }
}