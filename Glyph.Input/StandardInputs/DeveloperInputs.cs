using System.Collections.Generic;
using Glyph.Input.Composites;
using Glyph.Input.Handlers.Buttons;
using Microsoft.Xna.Framework.Input;

namespace Glyph.Input.StandardInputs
{
    public class DeveloperInputs : List<IInputHandler>
    {
        public const string XboxQuit = Prefix + "XboxQuit";
        public const string Fullscreen = Prefix + "Fullscreen";
        public const string StatusDisplay = Prefix + "StatusDisplay";
        public const string Mute = Prefix + "Mute";
        public const string ToogleSong = Prefix + "ToogleSong";
        public const string PreviousSong = Prefix + "PreviousSong";
        public const string NextSong = Prefix + "NextSong";
        private const string Prefix = "Dev-";

        public DeveloperInputs()
        {
            AddRange(new IInputHandler[]
            {
                new InputSimultaneous(XboxQuit)
                {
                    new PadButtonHandler("Button1", Buttons.Start, InputActivity.Pressed),
                    new PadButtonHandler("Button2", Buttons.Back, InputActivity.Pressed)
                },
                new KeyHandler(Fullscreen, Keys.F12),
                new KeyHandler(StatusDisplay, Keys.F11),
                new KeyHandler(Mute, Keys.F10),
                new KeyHandler(ToogleSong, Keys.F2),
                new KeyHandler(PreviousSong, Keys.F3),
                new KeyHandler(NextSong, Keys.F4)
            });
        }
    }
}