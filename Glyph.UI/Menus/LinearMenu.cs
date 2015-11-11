using System;
using System.Collections.Generic;
using Diese.Injection;
using Glyph.Composition;
using Glyph.Input;
using Glyph.Input.StandardInputs;
using Glyph.UI.Controls;

namespace Glyph.UI.Menus
{
    public class LinearMenu : GlyphObject, IMenu
    {
        private readonly InputManager _inputManager;
        private readonly List<IButton> _buttons;
        public int SelectedIndex { get; private set; }
        public int DefaultSelection { get; set; }
        public Axis NavigationAxis { get; set; }
        public bool NavigationLoop { get; set; }
        public event EventHandler<SelectionEventArgs> SelectionChanged;
        public event EventHandler<SelectionEventArgs> SelectionTriggered;

        public IButton SelectedControl
        {
            get
            {
                return SelectedIndex >= 0 && SelectedIndex < Count
                    ? _buttons[SelectedIndex]
                    : null;
            }
        }

        public LinearMenu(InputManager inputManager, IDependencyInjector injector)
            : base(injector)
        {
            _inputManager = inputManager;
            _buttons = new List<IButton>();

            NavigationAxis = Axis.Vertical;
            SelectedIndex = -1;
            NavigationLoop = true;

            Schedulers.Initialize.Plan(InitializeLocal);
            Schedulers.Update.Plan(HandleInput);
        }

        public void Add(IButton component)
        {
            base.Add(component);
            _buttons.Add(component);
            component.Entered += ButtonOnEntered;
            component.Triggered += ButtonOnTriggered;
        }

        public void Remove(IButton component)
        {
            component.Triggered -= ButtonOnTriggered;
            _buttons.Remove(component);
            base.Remove(component);
        }

        private void InitializeLocal()
        {
            SelectedIndex = -1;
        }

        private void HandleInput(ElapsedTime elapsedTime)
        {
            if (!_inputManager.IsMouseUsed)
            {
                if (_buttons.Count == 0)
                    SelectedIndex = -1;
                else if (SelectedIndex == -1)
                    SelectedIndex = DefaultSelection;
                else
                {
                    if (_inputManager[NavigationAxis == Axis.Vertical ? MenuInputs.Up : MenuInputs.Left])
                        SelectedIndex--;
                    else if (_inputManager[NavigationAxis == Axis.Vertical ? MenuInputs.Down : MenuInputs.Right])
                        SelectedIndex++;

                    if (SelectedIndex < 0)
                        SelectedIndex = NavigationLoop ? _buttons.Count - 1 : 0;
                    else if (SelectedIndex >= _buttons.Count)
                        SelectedIndex = NavigationLoop ? 0 : _buttons.Count - 1;
                }

                foreach (IButton button in _buttons)
                    button.Hover = false;

                _buttons[SelectedIndex].Hover = true;
            }

            if (_inputManager[MenuInputs.Cancel])
                TriggerSelection(DefaultSelection);
        }

        private void ButtonOnEntered(object sender, EventArgs eventArgs)
        {
            var button = sender as IButton;
            if (button == null)
                throw new InvalidCastException();

            if (SelectionChanged != null)
                SelectionChanged(this, new SelectionEventArgs(_buttons.IndexOf(button)));
        }

        private void ButtonOnTriggered(object sender, EventArgs eventArgs)
        {
            TriggerSelection(SelectedIndex);
        }

        private void TriggerSelection(int selection)
        {
            if (SelectionTriggered != null)
                SelectionTriggered(this, new SelectionEventArgs(selection));
        }

        IEnumerator<IButton> IEnumerable<IButton>.GetEnumerator()
        {
            return _buttons.GetEnumerator();
        }
    }
}