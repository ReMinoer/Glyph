using System;
using System.Collections.Generic;
using Diese.Injection;
using Fingear.MonoGame;
using Glyph.Composition;
using Glyph.Input;
using Glyph.Input.StandardControls;
using Glyph.UI.Controls;
using Diese.Collections;
using Glyph.Core;

namespace Glyph.UI.Menus
{
    public class LinearMenu : GlyphObject, IMenu
    {
        private readonly ControlManager _controlManager;
        private readonly List<IButton> _buttons;
        private readonly IReadOnlyCollection<IButton> _buttonsReadOnly;
        public int SelectedIndex { get; private set; }
        public int DefaultSelection { get; set; }
        public Axis NavigationAxis { get; set; }
        public bool NavigationLoop { get; set; }

        public IEnumerable<IButton> Buttons
        {
            get { return _buttonsReadOnly; }
        }

        public IButton SelectedControl
        {
            get
            {
                return SelectedIndex >= 0 && SelectedIndex < _buttons.Count
                    ? _buttons[SelectedIndex]
                    : null;
            }
        }

        public event EventHandler<SelectionEventArgs> SelectionChanged;
        public event EventHandler<SelectionEventArgs> SelectionTriggered;

        public LinearMenu(ControlManager controlManager, IDependencyInjector injector)
            : base(injector)
        {
            _controlManager = controlManager;
            _buttons = new List<IButton>();
            _buttonsReadOnly = new ReadOnlyCollection<IButton>(_buttons);

            NavigationAxis = Axis.Vertical;
            SelectedIndex = -1;
            NavigationLoop = true;

            Schedulers.Initialize.Plan(InitializeLocal).AtStart();
            Schedulers.Update.Plan(HandleInput).AtStart();
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
            bool isMouseUsed = _controlManager.InputSources.Any<MouseSource>();

            MenuControls menuControls;
            if (!isMouseUsed)
            {
                if (_buttons.Count == 0)
                    SelectedIndex = -1;
                else if (SelectedIndex == -1)
                    SelectedIndex = DefaultSelection;
                else
                {
                    if (_controlManager.TryGetLayer(out menuControls))
                    {
                        switch (NavigationAxis)
                        {
                            case Axis.Vertical:
                                if (menuControls.Up.IsActive())
                                    SelectedIndex--;
                                if (menuControls.Down.IsActive())
                                    SelectedIndex++;
                                break;
                            case Axis.Horizontal:
                                if (menuControls.Left.IsActive())
                                    SelectedIndex--;
                                if (menuControls.Right.IsActive())
                                    SelectedIndex++;
                                break;
                        }
                    }

                    if (SelectedIndex < 0)
                        SelectedIndex = NavigationLoop ? _buttons.Count - 1 : 0;
                    else if (SelectedIndex >= _buttons.Count)
                        SelectedIndex = NavigationLoop ? 0 : _buttons.Count - 1;
                }

                foreach (IButton button in _buttons)
                    button.Hover = false;

                _buttons[SelectedIndex].Hover = true;
            }

            if (_controlManager.TryGetLayer(out menuControls))
            {
                if (menuControls.Cancel.IsActive())
                    TriggerSelection(DefaultSelection);
            }
        }

        private void ButtonOnEntered(object sender, EventArgs eventArgs)
        {
            var button = sender as IButton;
            if (button == null)
                throw new InvalidCastException();

            SelectionChanged?.Invoke(this, new SelectionEventArgs(_buttons.IndexOf(button)));
        }

        private void ButtonOnTriggered(object sender, EventArgs eventArgs)
        {
            TriggerSelection(SelectedIndex);
        }

        private void TriggerSelection(int selection)
        {
            SelectionTriggered?.Invoke(this, new SelectionEventArgs(selection));
        }
    }
}