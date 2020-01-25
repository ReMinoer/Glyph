using System.Collections.Generic;
using System.Windows;
using Fingear.MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop.Input;

namespace Glyph.WpfInterop
{
    public class WpfInputStates : IInputStates
    {
        private readonly WpfKeyboard _keyboard;
        private readonly WpfMouse _mouse;
        private KeyboardState? _keyboardState;
        private MouseState? _mouseState;
        private Dictionary<PlayerIndex, GamePadState> _gamePadStates;
        public bool Ignored { get; private set; }

        public KeyboardState KeyboardState
        {
            get
            {
                if (_keyboardState == null)
                    _keyboardState = Ignored ? new KeyboardState() : _keyboard.GetState();
                return _keyboardState.Value;
            }
        }

        public MouseState MouseState
        {
            get
            {
                if (_mouseState == null)
                    _mouseState = Ignored ? new MouseState() : _mouse.GetState();
                return _mouseState.Value;
            }
        }

        public GamePadState this[PlayerIndex playerIndex]
        {
            get
            {
                GamePadState gamePadState;
                if (_gamePadStates == null)
                    _gamePadStates = new Dictionary<PlayerIndex, GamePadState>();
                else if (_gamePadStates.TryGetValue(playerIndex, out gamePadState))
                    return gamePadState;

                gamePadState = Ignored ? new GamePadState() : GamePad.GetState(playerIndex);
                _gamePadStates.Add(playerIndex, gamePadState);
                return gamePadState;
            }
        }

        public WpfInputStates(UIElement uiElement)
        {
            _keyboard = new WpfKeyboard(uiElement);
            _mouse = new WpfMouse(uiElement);
        }

        public void Clean()
        {
            _keyboardState = null;
            _mouseState = null;
            _gamePadStates?.Clear();

            Ignored = false;
        }

        public void Reset()
        {
            _mouse.Reset();
        }

        public void Ignore()
        {
            Ignored = true;
        }
    }
}