using Glyph.Input.StandardActions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Glyph.Input
{
    public class InputManager
    {
        public PlayerIndex PlayerIndex { get; private set; }
        public GameControls Controls { get; set; }
        public KeyboardState Keyboard { get; private set; }
        public MouseState Mouse { get; private set; }
        public GamePadState GamePad { get; private set; }
        public KeyboardState LastKeyboard { get; private set; }
        public MouseState LastMouse { get; private set; }
        public GamePadState LastGamePad { get; private set; }
        public bool IsGamePadUsed { get; set; }
        public bool IsMouseUsed { get; set; }
        public bool IsClicDownNow
        {
            get { return Mouse.LeftButton == ButtonState.Pressed && LastMouse.LeftButton != ButtonState.Pressed; }
        }
        public bool IsClicUpNow
        {
            get { return Mouse.LeftButton == ButtonState.Released && LastMouse.LeftButton != ButtonState.Released; }
        }
        public Vector2 MouseWindow { get { return new Vector2(Mouse.X, Mouse.Y) - Resolution.WindowMargin; } }
        public Vector2 MouseScreen { get { return MouseWindow / Resolution.ScaleRatio; } }
        public Vector2 MouseSpace
        {
            get
            {
                return MouseWindow / (Resolution.ScaleRatio * Camera.Zoom)
                       + new Vector2(Camera.VectorPosition.X, Camera.VectorPosition.Y);
            }
        }

        public InputManager(PlayerIndex index)
        {
            PlayerIndex = index;
            IsGamePadUsed = false;
            IsMouseUsed = false;

            Controls = new GameControls {new DeveloperActions()};
        }

        public void Update(bool isGameActive)
        {
            if (isGameActive)
            {
                LastKeyboard = Keyboard;
                LastMouse = Mouse;
                LastGamePad = GamePad;

                Keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
                Mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
                GamePad = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex);

                if (GamePad != LastGamePad)
                {
                    IsGamePadUsed = true;
                    IsMouseUsed = false;
                }
                else if (Keyboard != LastKeyboard)
                {
                    IsGamePadUsed = false;
                    IsMouseUsed = false;
                }
                else if (Mouse != LastMouse)
                {
                    IsGamePadUsed = false;
                    IsMouseUsed = true;
                }
            }
            else
            {
                LastKeyboard = new KeyboardState();
                LastMouse = new MouseState();
                LastGamePad = new GamePadState();

                Keyboard = new KeyboardState();
                Mouse = new MouseState();
                GamePad = new GamePadState();
            }
        }

        public bool IsActionPressed(string name)
        {
            return Controls.ContainsKey(name) && Controls[name].IsPressed(this);
        }

        public bool IsActionDownNow(string name)
        {
            return Controls.ContainsKey(name) && Controls[name].IsDownNow(this);
        }

        public bool IsKeyDownNow(Keys key)
        {
            return Keyboard.IsKeyDown(key) && !LastKeyboard.IsKeyDown(key);
        }

        public bool IsKeyUpNow(Keys key)
        {
            return Keyboard.IsKeyUp(key) && !LastKeyboard.IsKeyUp(key);
        }
    }
}