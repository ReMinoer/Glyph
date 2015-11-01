using System;
using System.Collections.Generic;
using System.Linq;

namespace Glyph.Composition.Layers
{
    public class LayerManager<TLayer> : GlyphComponent, IUpdate
        where TLayer : class, ILayer<TLayer>
    {
        private readonly Dictionary<LayerRoot<TLayer>, TLayer> _layers;
        public Func<LayerRoot<TLayer>, TLayer> LayerFactory { get; set; }

        public IReadOnlyCollection<TLayer> Layers
        {
            get { return _layers.Values.ToList().AsReadOnly(); }
        }

        public LayerManager(Func<LayerRoot<TLayer>, TLayer> layerFactory)
        {
            LayerFactory = layerFactory;
            _layers = new Dictionary<LayerRoot<TLayer>, TLayer>();
        }

        public override void Initialize()
        {
            foreach (TLayer layer in _layers.Values)
                layer.Initialize();
        }

        internal TLayer Add(LayerRoot<TLayer> layerRoot)
        {
            TLayer layer = LayerFactory(layerRoot);
            _layers.Add(layerRoot, layer);

            return layer;
        }

        internal void Remove(LayerRoot<TLayer> layerRoot)
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