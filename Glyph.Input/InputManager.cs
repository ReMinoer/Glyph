using System;
using Glyph.Input.StandardInputs;

namespace Glyph.Input
{
    public class InputManager
    {
        public GameControls Controls { get; set; }
        public InputStates InputStates { get; private set; }
        public bool IsGamePadUsed { get; private set; }
        public bool IsMouseUsed { get; private set; }

        public InputManager()
        {
            IsGamePadUsed = false;
            IsMouseUsed = false;

            InputStates = new InputStates();

            Controls = new GameControls
            {
                new DeveloperInputs(),
                new MouseInputs()
            };
        }

        public void Update(bool isGameActive)
        {
            InputStates.Update(isGameActive);

            foreach (IInputHandler inputHandler in Controls.Values)
                inputHandler.Update(InputStates);
        }

        public bool this[object name]
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

            if (Controls[name].IsActivated)
                RefreshDeviceUsage(Controls[name].InputSource);

            return inputHandler.Value;
        }

        private void RefreshDeviceUsage(InputSource inputSource)
        {
            switch (inputSource)
            {
                case InputSource.GamePad:
                    IsGamePadUsed = true;
                    IsMouseUsed = false;
                    break;
                case InputSource.Keyboard:
                    IsGamePadUsed = false;
                    IsMouseUsed = false;
                    break;
                case InputSource.Mouse:
                    IsGamePadUsed = false;
                    IsMouseUsed = true;
                    break;
            }
        }
    }
}