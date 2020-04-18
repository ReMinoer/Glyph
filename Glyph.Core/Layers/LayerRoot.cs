using Glyph.Composition;

namespace Glyph.Core.Layers
{
    public class LayerRoot<TLayer> : GlyphComponent, ILayerRoot<TLayer>
        where TLayer : class, ILayer
    {
        private readonly LayerManager<TLayer> _layerManager;
        public TLayer Layer { get; }
        ILayer ILayerRoot.Layer => Layer;

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