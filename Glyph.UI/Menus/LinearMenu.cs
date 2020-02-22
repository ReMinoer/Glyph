using System;
using System.Collections.Generic;
using Glyph.UI.Controls;
using Diese.Collections.ReadOnly;
using Glyph.Core;

namespace Glyph.UI.Menus
{
    public class LinearMenu : GlyphObject, IMenu
    {
        private readonly List<IButton> _buttons;
        private readonly IReadOnlyCollection<IButton> _buttonsReadOnly;
        public int SelectedIndex { get; private set; }
        public int DefaultSelection { get; set; }
        public Axis NavigationAxis { get; set; }
        public bool NavigationLoop { get; set; }

        public IEnumerable<IButton> Buttons => _buttonsReadOnly;
        public IButton SelectedControl =>
            SelectedIndex >= 0 && SelectedIndex < _buttons.Count
                ? _buttons[SelectedIndex]
                : null;

        public event EventHandler<SelectionEventArgs> SelectionChanged;
        public event EventHandler<SelectionEventArgs> SelectionTriggered;

        public LinearMenu(GlyphResolveContext context)
            : base(context)
        {
            _buttons = new List<IButton>();
            _buttonsReadOnly = new ReadOnlyCollection<IButton>(_buttons);

            NavigationAxis = Axis.Vertical;
            SelectedIndex = -1;
            NavigationLoop = true;

            var userInterface = Add<UserInterface>();
            userInterface.DirectionChanged += OnDirectionChanged;
            userInterface.Cancelled += OnCancelled;

            Schedulers.Initialize.Plan(InitializeLocal).AtStart();
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

        private void OnDirectionChanged(object sender, HandlableDirectionEventArgs e)
        {
            switch (NavigationAxis)
            {
                case Axis.Vertical:
                    switch (e.Direction.ToVertical())
                    {
                        case Vertical.Up:
                            e.Handle();
                            SelectedIndex--;
                            break;
                        case Vertical.Down:
                            e.Handle();
                            SelectedIndex++;
                            break;
                    }
                    break;
                case Axis.Horizontal:
                    switch (e.Direction.ToHorizontal())
                    {
                        case Horizontal.Left:
                            e.Handle();
                            SelectedIndex--;
                            break;
                        case Horizontal.Right:
                            e.Handle();
                            SelectedIndex++;
                            break;
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }

            if (!e.IsHandled)
                return;

            if (SelectedIndex < 0)
                SelectedIndex = NavigationLoop ? _buttons.Count - 1 : 0;
            else if (SelectedIndex >= _buttons.Count)
                SelectedIndex = NavigationLoop ? 0 : _buttons.Count - 1;

            foreach (IButton button in _buttons)
                button.Hover = false;
            _buttons[SelectedIndex].Hover = true;
        }

        private void OnCancelled(object sender, HandlableEventArgs e)
        {
            e.Handle();
            TriggerSelection(DefaultSelection);
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