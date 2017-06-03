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
        private readonly List<IControl> _controls = new List<IControl>();
        private readonly HashSet<IInputSource> _sources = new HashSet<IInputSource>();
        public IEnumerable<IInput> Inputs => Layers.SelectMany(x => x).SelectMany(x => x.Inputs);
        public ObservableCollection<IControlLayer> Layers { get; } = new ObservableCollection<IControlLayer>();
        public IReadOnlyCollection<IInputSource> InputSources { get; private set; }
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
                    _inputClient.States.Clean();

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
            InputSources = Diese.Collections.ReadOnlyCollection<IInputSource>.Empty;
        }

        public void Update(ElapsedTime elapsedTime, bool isGameActive)
        {
            if (!isGameActive)
            {
                InputClient?.States.Ignore();
                return;
            }

            InputClient?.States.Clean();
            _controls.Clear();
            _sources.Clear();

            InputManager.Instance.Update();

            _controls.AddRange(Layers.Where(x => x.Enabled).SelectMany(x => x));
            if (_controls.Count == 0)
                return;
            
            foreach (IControl control in _controls)
                control.Update(elapsedTime.UnscaledDelta);

            foreach (IInputSource source in _controls.Where(control => control.IsActive()).SelectMany(control => control.Sources))
                _sources.Add(source);

            if (InputSources.SetEquals(_sources))
                return;

            InputSources = _sources.ToArray().AsReadOnly();
            InputSourcesChanged?.Invoke(InputSources);
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