using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Diese.Collections;
using Fingear;
using Fingear.MonoGame;

namespace Glyph.Input
{
    public class ControlManager
    {
        private readonly Dictionary<object, IControlLayer> _layers;
        private readonly IReadOnlyDictionary<object, IControlLayer> _layersReadOnly;
        public IEnumerable<IInputSource> InputSources { get; private set; }
        public event Action<IEnumerable<IInputSource>> InputSourcesChanged;

        public IControlLayer this[object key] => _layers[key];
        public IEnumerable<IControlLayer> Layers => _layersReadOnly.Values;

        public ControlManager()
        {
            InputSources = Enumerable.Empty<IInputSource>();
            _layers = new Dictionary<object, IControlLayer>();
            _layersReadOnly = new ReadOnlyDictionary<object, IControlLayer>(_layers);

            InputStates.Instance.Reset();
        }

        public void Update(ElapsedTime elapsedTime, bool isGameActive)
        {
            if (!isGameActive)
            {
                InputStates.Instance.Reset();
                return;
            }

            InputStates.Instance.Update();

            IControl[] controls = _layers.Values.SelectMany(x => x).ToArray();
            foreach (IControl control in controls)
                control.Update(elapsedTime.UnscaledDelta);

            IInputSource[] sources = controls.Where(control => control.IsActive()).SelectMany(control => control.Sources).Distinct().ToArray();
            if (InputSources.SetEquals(sources))
                return;

            InputSources = sources;
            InputSourcesChanged?.Invoke(InputSources);
        }

        public void AddLayer(object key, IControlLayer layer)
        {
            if (_layers.ContainsKey(key))
                throw new ArgumentException();

            _layers.Add(key, layer);
        }

        public void RemoveLayer(object key)
        {
            _layers.Remove(key);
        }

        public bool ContainsLayer(object key)
        {
            return _layers.ContainsKey(key);
        }

        public bool TryGetLayer<T>(out T layer)
            where T : class, IControlLayer
        {
            return _layers.Values.Any(out layer);
        }

        public bool TryGetLayer<T>(object key, out T layer)
            where T : class, IControlLayer
        {
            layer = _layers[key] as T;
            return layer != null;
        }
    }
}