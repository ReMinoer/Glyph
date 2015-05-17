using System;
using System.Collections.Generic;
using Glyph.Input;
using Glyph.Input.StandardInputs;
using Glyph.UI.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.UI.Menus
{
    public class ButtonsMenu
    {
        public List<Button> Buttons { get; set; }
        public int Selection { get; set; }
        public int DefaultSelection { get; set; }
        public event EventHandler<MenuEventArgs> Selected;

        public ButtonsMenu()
        {
            Buttons = new List<Button>();
        }

        public virtual void Initialize()
        {
            Selection = -1;
            foreach (Button button in Buttons)
                button.Initialize();
        }

        public virtual void Add(Button button)
        {
            Buttons.Add(button);
            button.Activated += ButtonOnActivated;
        }

        public virtual void AddCollection(IEnumerable<Button> buttons)
        {
            foreach (Button button in buttons)
                Add(button);
        }

        public virtual void LoadContent(ContentLibrary content)
        {
            foreach (Button button in Buttons)
                button.LoadContent(content);
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (Button button in Buttons)
                button.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Button button in Buttons)
                button.Draw(spriteBatch);
        }

        public virtual void HandleInput(InputManager input)
        {
            if (input.IsMouseUsed)
            {
                Selection = -1;
                for (var i = 0; i < Buttons.Count; i++)
                    if (Buttons[i].Enable)
                        Selection = i;
            }
            else
            {
                if (Buttons.Count == 0)
                    Selection = -1;
                else if (Selection == -1)
                    Selection = DefaultSelection;
                else
                {
                    if (input[MenuInputs.Up])
                        Selection--;
                    else if (input[MenuInputs.Down])
                        Selection++;

                    if (Selection < 0)
                        Selection = Buttons.Count - 1;
                    else if (Selection >= Buttons.Count)
                        Selection = 0;
                }

                foreach (Button b in Buttons)
                    b.Enable = false;
                Buttons[Selection].Enable = true;
            }

            foreach (Button bouton in Buttons)
                bouton.HandleInput(input);

            if (input[MenuInputs.Cancel])
                ButtonOnActivated(DefaultSelection);
        }

        protected virtual void ButtonOnActivated(int selection)
        {
            if (Selected != null)
                Selected(this, new MenuEventArgs(selection));
        }

        private void ButtonOnActivated(object sender, EventArgs eventArgs)
        {
            ButtonOnActivated(Selection);
        }
    }

    public class MenuEventArgs : EventArgs
    {
        public int Selection { get; set; }

        public MenuEventArgs(int selection)
        {
            Selection = selection;
        }
    }
}