using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Glyph.Input
{
    public class InputStates
    {
        private readonly Dictionary<PlayerIndex, GamePadState> _gamePadStates;
        private readonly PlayerIndex[] _padPlayerIndexes;
        public KeyboardState KeyboardState { get; private set; }
        public MouseState MouseState { get; private set; }

        public ReadOnlyDictionary<PlayerIndex, GamePadState> GamePadStates
        {
            get { return new ReadOnlyDictionary<PlayerIndex, GamePadState>(_gamePadStates); }
        }

        public InputStates()
        {
            _gamePadStates = new Dictionary<PlayerIndex, GamePadState>();
            _padPlayerIndexes = Enum.GetValues(typeof(PlayerIndex)) as PlayerIndex[];

            if (_padPlayerIndexes == null)
                return;

            foreach (PlayerIndex playerIndex in _padPlayerIndexes)
                _gamePadStates[playerIndex] = new GamePadState();
        }

        public void Update(bool isGameActive)
        {
            if (isGameActive)
            {
                KeyboardState = Keyboard.GetState();
                MouseState = Mouse.GetState();
                foreach (PlayerIndex playerIndex in _padPlayerIndexes)
                    _gamePadStates[playerIndex] = GamePad.GetState(playerIndex);
            }
            else
            {
                KeyboardState = new KeyboardState();
                MouseState = new MouseState();
                foreach (PlayerIndex playerIndex in _padPlayerIndexes)
                    _gamePadStates[playerIndex] = new GamePadState();
            }
        }
    }
}