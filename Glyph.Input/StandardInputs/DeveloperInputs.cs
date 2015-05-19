using System.Collections.Generic;
using Glyph.Input.Composites;
using Glyph.Input.Handlers.Buttons;
using Microsoft.Xna.Framework.Input;

namespace Glyph.Input.StandardInputs
{
    public class DeveloperInputs : List<IInputHandler>
    {
        private const string Prefix = "dev-";
        public const string XboxQuit = Prefix + "XboxQuit";
        public const string Fullscreen = Prefix + "Fullscreen";
        public const string StatusDisplay = Prefix + "StatusDisplay";
        public const string Mute = Prefix + "Mute";
        public const string ToogleSong = Prefix + "ToogleSong";
        public const string PreviousSong = Prefix + "PreviousSong";
        public const string NextSong = Prefix + "NextSong";
        public const string Clic = Prefix + "Clic";
        public const string ClicRelease = Prefix + "ClicRelease";

        public DeveloperInputs()
        {
            var xboxQuit = new InputSimultaneous(XboxQuit)
            {
                new PadButtonHandler("XboxQuit1", Buttons.Start, InputAction.Pressed),
                new PadButtonHandler("XboxQuit2", Buttons.Back, InputAction.Pressed)
            };

            var fullscreen = new KeyHandler(Fullscreen, Keys.F12);
            var statusDisplay = new KeyHandler(StatusDisplay, Keys.F11);
            var mute = new KeyHandler(Mute, Keys.F10);

            var toogleSong = new KeyHandler(ToogleSong, Keys.F2);
            var previousSong = new KeyHandler(PreviousSong, Keys.F3);
            var nextSong = new KeyHandler(NextSong, Keys.F4);

            var clic = new MouseButtonHandler(Clic, MouseButton.Left);
            var clicRelease = new MouseButtonHandler(ClicRelease, MouseButton.Left, InputAction.Released);

            Add(xboxQuit);
            Add(fullscreen);
            Add(statusDisplay);
            Add(mute);
            Add(toogleSong);
            Add(previousSong);
            Add(nextSong);
            Add(clic);
            Add(clicRelease);
        }
    }
}