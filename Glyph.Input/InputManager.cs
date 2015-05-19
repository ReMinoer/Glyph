using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Glyph.Input.StandardInputs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Glyph.Input
{
    public class InputManager
    {
        private readonly Dictionary<PlayerIndex, GamePadState> _gamePadStates;
        private readonly PlayerIndex[] _padPlayerIndexes;
        public GameControls Controls { get; set; }
        public KeyboardState KeyboardState { get; private set; }
        public MouseState MouseState { get; private set; }
        
        public bool IsGamePadUsed { get; private set; }
        public bool IsMouseUsed { get; private set; }

        public ReadOnlyDictionary<PlayerIndex, GamePadState> GamePadStates
        {
            get { return new ReadOnlyDictionary<PlayerIndex, GamePadState>(_gamePadStates); }
        }

        public Vector2 MouseInWindow
        {
            get { return new Vector2(MouseState.X, MouseState.Y) - Resolution.WindowMargin; }
        }

        public Vector2 MouseInScreen
        {
            get { return MouseInWindow / Resolution.ScaleRatio; }
        }

        public Vector2 MouseInSpace
        {
            get
            {
                return MouseInWindow / (Resolution.ScaleRatio * Camera.Zoom)
                       + new Vector2(Camera.VectorPosition.X, Camera.VectorPosition.Y);
            }
        }

        public InputManager()
        {
            IsGamePadUsed = false;
            IsMouseUsed = false;

            _gamePadStates = new Dictionary<PlayerIndex, GamePadState>();
            _padPlayerIndexes = Enum.GetValues(typeof(PlayerIndex)) as PlayerIndex[];

            if (_padPlayerIndexes != null)
                foreach (PlayerIndex playerIndex in _padPlayerIndexes)
                    _gamePadStates[playerIndex] = new GamePadState();

            Controls = new GameControls {new DeveloperInputs()};
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

            foreach (IInputHandler inputHandler in Controls.Values)
                inputHandler.Update(this);
        }

        public bool this[string name]
        {
            get
            {
                if (!Controls.ContainsKey(name))
                    return false;

                if (Controls[name].IsActivated)
                    RefreshDeviceUsage(Controls[name].InputSource);

                return Controls[name].IsActivated;
            }
        }

        public T GetValue<T>(string name)
        {
            var inputHandler = Controls[name] as IInputHandler<T>;
            if (inputHandler == null)
                throw new ArgumentException(string.Format("\"{0}\" doesn't return a value of type {1} !", name,
                    typeof(T)));

            return inputHandler.Value;
        }

        private void RefreshDeviceUsage(InputSource inputSource)
        {
            if (inputSource == InputSource.GamePad)
            {
                IsGamePadUsed = true;
                IsMouseUsed = false;
            }
            else if (inputSource == InputSource.Keyboard)
            {
                IsGamePadUsed = false;
                IsMouseUsed = false;
            }
            else if (inputSource == InputSource.Mouse)
            {
                IsGamePadUsed = false;
                IsMouseUsed = true;
            }
        }
    }
}