namespace Glyph.Core.Layers
{
    public abstract class LayerBase : ILayer
    {
        public ILayerRoot Root { get; private set; }
        public virtual int Index { get; set; }

        protected LayerBase(ILayerRoot root)
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