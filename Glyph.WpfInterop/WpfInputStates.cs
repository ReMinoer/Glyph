using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly Dictionary<PlayerIndex, GamePadState> _gamePadStates;
        private readonly PlayerIndex[] _padPlayerIndexes;
        public KeyboardState KeyboardState { get; private set; }
        public MouseState MouseState { get; private set; }
        public ReadOnlyDictionary<PlayerIndex, GamePadState> GamePadStates { get; private set; }

        public WpfInputStates(UIElement uiElement)
        {
            _keyboard = new WpfKeyboard(uiElement);
            _mouse = new WpfMouse(uiElement);

            _gamePadStates = new Dictionary<PlayerIndex, GamePadState>();
            _padPlayerIndexes = Enum.GetValues(typeof(PlayerIndex)) as PlayerIndex[];
            if (_padPlayerIndexes == null)
                return;

            foreach (PlayerIndex playerIndex in _padPlayerIndexes)
                _gamePadStates[playerIndex] = new GamePadState();

            GamePadStates = new ReadOnlyDictionary<PlayerIndex, GamePadState>(_gamePadStates);

            Reset();
        }

        public void Update()
        {
            KeyboardState = _keyboard.GetState();
            MouseState = _mouse.GetState();
            foreach (PlayerIndex playerIndex in _padPlayerIndexes)
                _gamePadStates[playerIndex] = GamePad.GetState(playerIndex);
        }

        public void Reset()
        {
            KeyboardState = new KeyboardState();
            MouseState = new MouseState();
            foreach (PlayerIndex playerIndex in _padPlayerIndexes)
                _gamePadStates[playerIndex] = new GamePadState();
        }
    }
}