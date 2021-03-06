﻿using System;
using Fingear.Inputs;
using Fingear.MonoGame;
using IInputStates = Fingear.MonoGame.IInputStates;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Glyph.Core.Inputs
{
    public class InputClientManager
    {
        private IInputClient _current;
        private IDrawClient _drawClient;

        public IInputClient InputClient
        {
            get => _current;
            set
            {
                if (_current == value)
                    return;
                
                _current = value;

                IInputStates inputStates = _current?.States;
                inputStates?.Clean();

                InputSystem.Instance.InputStates = inputStates;

                InputManager.Instance.InputStates = inputStates;
                InputManager.Instance.Reset();
                     
                InputClientChanged?.Invoke(_current);
            }
        }

        public IDrawClient DrawClient
        {
            get => _drawClient;
            set
            {
                if (_drawClient == value)
                    return;

                if (_drawClient != null)
                    _drawClient.SizeChanged -= OnResolutionChanged;

                _drawClient = value;

                if (_drawClient != null)
                {
                    InputSystem.Instance.Mouse.Cursor.DefaultValue = (_drawClient.Size / 2).AsSystemVector();
                    _drawClient.SizeChanged += OnResolutionChanged;
                }

                DrawClientChanged?.Invoke(_drawClient);
            }
        }

        public event Action<IInputClient> InputClientChanged;
        public event Action<IDrawClient> DrawClientChanged;

        private void OnResolutionChanged(Vector2 size)
        {
            Vector2 scale = size / _drawClient.Size;
            InputSystem.Instance.Mouse.Cursor.DefaultValue = InputSystem.Instance.Mouse.Cursor.DefaultValue * scale.AsSystemVector();
        }
    }
}