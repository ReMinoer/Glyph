using System;
using Fingear;
using Fingear.MonoGame;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Glyph.Core.Inputs
{
    public class InputClientManager
    {
        private IInputClient _current;
        public CursorControls CursorControls { get; }

        public IInputClient Current
        {
            get => _current;
            set
            {
                if (_current == value)
                    return;

                if (_current != null)
                    _current.Resolution.SizeChanged -= OnResolutionChanged;

                _current = value;

                if (_current != null)
                {
                    _current.States.Clean();

                    InputSystem.Instance.Mouse.Cursor.DefaultValue = (_current.Resolution.WindowSize / 2).AsSystemVector();
                    _current.Resolution.SizeChanged += OnResolutionChanged;
                }

                InputSystem.Instance.InputStates = _current?.States;
                InputManager.Instance.InputStates = _current?.States;

                ClientChanged?.Invoke(_current);
            }
        }

        public event Action<IInputClient> ClientChanged;

        public InputClientManager()
        {
            CursorControls = new CursorControls(this);
        }

        private void OnResolutionChanged(Vector2 size)
        {
            Vector2 scale = size / _current.Resolution.WindowSize;
            InputSystem.Instance.Mouse.Cursor.DefaultValue = InputSystem.Instance.Mouse.Cursor.DefaultValue * scale.AsSystemVector();
        }
    }
}