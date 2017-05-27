using Fingear;
using Fingear.Controls;
using Fingear.Controls.Composites;
using Fingear.Controls.Containers;
using Fingear.MonoGame;
using Fingear.MonoGame.Inputs;
using Glyph.Core.Controls;
using Glyph.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Vector2 = System.Numerics.Vector2;

namespace Glyph.Core.ControlLayers
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
            MonoGameInputSytem inputSystem = MonoGameInputSytem.Instance;
            KeyboardSource keyboard = inputSystem.Keyboard;
            GamePadSource gamePad = inputSystem[PlayerIndex.One];

            var xboxQuit = new ControlSimultaneous<IControl>("XboxQuit");
            xboxQuit.Add(new Control(gamePad[GamePadButton.Start]));
            xboxQuit.Add(new Control(gamePad[GamePadButton.Back]));
            Add(XboxQuit = xboxQuit);

            Add(Fullscreen = new Control("Fullscreen", keyboard[Keys.F12]));
            Add(StatusDisplay = new Control("StatusDisplay", keyboard[Keys.F11]));
            Add(Mute = new Control("Mute", keyboard[Keys.F10]));

            Add(CompositionLog = new Control("CompositionLog", keyboard[Keys.F9]));
            Add(UpdateSnapshot = new Control("UpdateSnapshot", keyboard[Keys.F8]));

            Add(ToogleSong = new Control("ToggleSong", keyboard[Keys.F2]));
            Add(PreviousSong = new Control("PreviousSong", keyboard[Keys.F3]));
            Add(NextSong = new Control("NextSong", keyboard[Keys.F4]));
        }
    }
}