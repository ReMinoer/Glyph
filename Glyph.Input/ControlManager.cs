using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Diese.Collections;
using Fingear;
using Fingear.MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Glyph.Input
{
    public class ControlManager
    {
        private IInputClient _inputClient;
        private readonly Dictionary<object, IControlLayer> _layers;
        private readonly IReadOnlyDictionary<object, IControlLayer> _layersReadOnly;
        public IEnumerable<IInputSource> InputSources { get; private set; }
        public IControlLayer this[object key] => _layers[key];
        public IEnumerable<IControlLayer> Layers => _layersReadOnly.Values;
        public Point DefaultMousePosition { get; set; }

        public IInputClient InputClient
        {
            get { return _inputClient; }
            set
            {
                if (_inputClient == value)
                    return;

                if (_inputClient != null)
                    _inputClient.Resolution.SizeChanged -= OnResolutionChanged;

                _inputClient = value;

                if (_inputClient != null)
                {
                    _inputClient.States.Reset();

                    DefaultMousePosition = (_inputClient.Resolution.WindowSize / 2).ToPoint();
                    _inputClient.Resolution.SizeChanged += OnResolutionChanged;
                }

                MonoGameInputSytem.Instance.InputStates = _inputClient?.States;

                InputClientChanged?.Invoke(_inputClient);
            }
        }

        public event Action<IEnumerable<IInputSource>> InputSourcesChanged;
        public event Action<IInputClient> InputClientChanged;

        public ControlManager()
        {
            InputSources = Enumerable.Empty<IInputSource>();
            _layers = new Dictionary<object, IControlLayer>();
            _layersReadOnly = new ReadOnlyDictionary<object, IControlLayer>(_layers);
        }

        public void Update(ElapsedTime elapsedTime, bool isGameActive)
        {
            if (!isGameActive)
            {
                InputClient?.States.Reset();
                return;
            }

            InputClient?.States.Update();

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

        public void ResetMouse()
        {
            Mouse.SetPosition(DefaultMousePosition.X, DefaultMousePosition.Y);
        }

        public void SetMousePosition(Point position)
        {
            Mouse.SetPosition(position.X, position.Y);
        }

        private void OnResolutionChanged(Vector2 size)
        {
            Vector2 scale = size / _inputClient.Resolution.WindowSize;
            DefaultMousePosition = DefaultMousePosition.ToVector2().Multiply(scale.X, scale.Y).ToPoint();
        }
    }
}