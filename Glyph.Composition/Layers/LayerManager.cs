using System;
using System.Collections.Generic;
using System.Linq;

namespace Glyph.Composition.Layers
{
    public class LayerManager<TLayer> : GlyphComponent, ILayerManager
        where TLayer : class, ILayer<TLayer>
    {
        protected readonly Dictionary<ILayerRoot<TLayer>, TLayer> _layers;
        public Func<ILayerRoot<TLayer>, TLayer> LayerFactory { get; set; }

        public IReadOnlyCollection<TLayer> Layers
        {
            get { return _layers.Values.ToList().AsReadOnly(); }
        }

        public LayerManager()
        {
            _layers = new Dictionary<ILayerRoot<TLayer>, TLayer>();
        }

        public LayerManager(Func<ILayerRoot<TLayer>, TLayer> layerFactory)
            : this()
        {
            LayerFactory = layerFactory;
        }

        public override void Initialize()
        {
            foreach (TLayer layer in _layers.Values)
                layer.Initialize();
        }

        public void Update(ElapsedTime elapsedTime)
        {
            foreach (TLayer layer in _layers.Values)
                layer.Update(elapsedTime);
        }

        public ILayerRoot<TLayer> GetLayerRoot(IGlyphComponent component)
        {
            foreach (ILayerRoot<TLayer> layerRoot in _layers.Keys)
                if (layerRoot.ContainsInChildren(component))
                    return layerRoot;

            return null;
        }

        protected internal virtual TLayer Add(ILayerRoot<TLayer> layerRoot)
        {
            TLayer layer = LayerFactory(layerRoot);
            _layers.Add(layerRoot, layerRoot.Layer);
            return layer;
        }

        protected internal virtual void Remove(ILayerRoot<TLayer> layerRoot)
        {
            _layers.Remove(layerRoot);
        }

        ILayerRoot ILayerManager.GetLayerRoot(IGlyphComponent component)
        {
            return GetLayerRoot(component);
        }
    }
}