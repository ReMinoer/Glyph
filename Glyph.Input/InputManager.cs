using System;
using Glyph.Input.StandardInputs;

namespace Glyph.Input
{
    public class InputManager
    {
        static private InputManager _instance;

        static public InputManager Instance
        {
            get { return _instance ?? (_instance = new InputManager()); }
        }

        public GameControls Controls { get; set; }
        public InputStates InputStates { get; private set; }
        public bool IsGamePadUsed { get; private set; }
        public bool IsMouseUsed { get; private set; }

        protected InputManager()
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

            if (Controls[name].IsActivated)
                RefreshDeviceUsage(Controls[name].InputSource);

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