using Fingear;
using Fingear.Controls;
using Fingear.Controls.Composites;
using Fingear.MonoGame.Inputs;
using Microsoft.Xna.Framework.Input;

namespace Glyph.Input.StandardControls
{
    public class DeveloperControls : ControlLayerBase
    {
        public IControl XboxQuit { get; }
        public IControl Fullscreen { get; }
        public IControl StatusDisplay { get; }
        public IControl Mute { get; }
        public IControl CompositionLog { get; }
        public IControl UpdateSnapshot { get; }
        public IControl ToogleSong { get; }
        public IControl PreviousSong { get; }
        public IControl NextSong { get; }

        public DeveloperControls()
        {
            var xboxQuit = new ControlSimultaneous<IControl>("XboxQuit");
            xboxQuit.Add(new Control(new GamePadButtonInput(GamePadButton.Start)));
            xboxQuit.Add(new Control(new GamePadButtonInput(GamePadButton.Back)));
            Add(XboxQuit = xboxQuit);

            Add(Fullscreen = new Control("Fullscreen", new KeyInput(Keys.F12)));
            Add(StatusDisplay = new Control("StatusDisplay", new KeyInput(Keys.F11)));
            Add(Mute = new Control("Mute", new KeyInput(Keys.F10)));

            Add(CompositionLog = new Control("CompositionLog", new KeyInput(Keys.F9)));
            Add(UpdateSnapshot = new Control("UpdateSnapshot", new KeyInput(Keys.F8)));

            Add(ToogleSong = new Control("ToggleSong", new KeyInput(Keys.F2)));
            Add(PreviousSong = new Control("PreviousSong", new KeyInput(Keys.F3)));
            Add(NextSong = new Control("NextSong", new KeyInput(Keys.F4)));
        }
    }
}