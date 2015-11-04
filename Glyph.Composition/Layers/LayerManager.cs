using System;
using System.Collections.Generic;
using System.Linq;

namespace Glyph.Composition.Layers
{
    public class LayerManager<TLayer> : GlyphComponent, IUpdate
        where TLayer : class, ILayer<TLayer>
    {
        private readonly Dictionary<ILayerRoot<TLayer>, TLayer> _layers;
        public Func<ILayerRoot<TLayer>, TLayer> LayerFactory { get; set; }

        public IReadOnlyCollection<TLayer> Layers
        {
            get { return _layers.Values.ToList().AsReadOnly(); }
        }

        public LayerManager(Func<ILayerRoot<TLayer>, TLayer> layerFactory)
        {
            LayerFactory = layerFactory;
            _layers = new Dictionary<ILayerRoot<TLayer>, TLayer>();
        }

        public override void Initialize()
        {
            foreach (TLayer layer in _layers.Values)
                layer.Initialize();
        }

        internal TLayer Add(ILayerRoot<TLayer> layerRoot)
        {
            TLayer layer = LayerFactory(layerRoot);
            _layers.Add(layerRoot, layer);

            return layer;
        }

        internal void Remove(ILayerRoot<TLayer> layerRoot)
        {
            _layers.Remove(layerRoot);
        }

        public void Update(ElapsedTime elapsedTime)
        {
            foreach (TLayer layer in _layers.Values)
                layer.Update(elapsedTime);
        }
    }
}