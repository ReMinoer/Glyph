namespace Glyph.Composition.Layers
{
    public class LayerRoot<TLayer> : GlyphComponent
        where TLayer : class, ILayer<TLayer>
    {
        private readonly LayerManager<TLayer> _layerManager;
        public TLayer Layer { get; private set; }

        public LayerRoot(LayerManager<TLayer> layerManager)
        {
            _layerManager = layerManager;
            Layer = _layerManager.Add(this);
        }

        public override void Dispose()
        {
            base.Dispose();
            _layerManager.Remove(this);
        }
    }
}