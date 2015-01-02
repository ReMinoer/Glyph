using Microsoft.Xna.Framework.Input;

namespace Glyph.Input.StandardActions
{
    public class DeveloperActions : ActionsCollection
    {
        private const string Prefix = "dev-";

        public const string XboxQuit = Prefix + "XboxQuit";
        public const string Fullscreen = Prefix + "Fullscreen";
        public const string StatusDisplay = Prefix + "StatusDisplay";
        public const string Mute = Prefix + "Mute";
        public const string ToogleSong = Prefix + "ToogleSong";
        public const string PreviousSong = Prefix + "PreviousSong";
        public const string NextSong = Prefix + "NextSong";

        public DeveloperActions()
        {
            var xboxQuit = new CombinedActionButton(XboxQuit);
            var fullscreen = new ActionButton(Fullscreen);
            var statusDisplay = new ActionButton(StatusDisplay);
            var mute = new ActionButton(Mute);
            var toogleSong = new ActionButton(ToogleSong);
            var previousSong = new ActionButton(PreviousSong);
            var nextSong = new ActionButton(NextSong);

            var xboxQuit1 = new ActionButton("xboxQuit1");
            var xboxQuit2 = new ActionButton("xboxQuit2");

            xboxQuit1.Buttons.Add(Buttons.Start);
            xboxQuit2.Buttons.Add(Buttons.Back);

            fullscreen.Keys.Add(Keys.F12);
            statusDisplay.Keys.Add(Keys.F11);
            mute.Keys.Add(Keys.F10);

            toogleSong.Keys.Add(Keys.F2);
            previousSong.Keys.Add(Keys.F3);
            nextSong.Keys.Add(Keys.F4);

            xboxQuit.ActionButtons.Add(xboxQuit1);
            xboxQuit.ActionButtons.Add(xboxQuit2);

            Add(xboxQuit);
            Add(fullscreen);
            Add(statusDisplay);
            Add(mute);
            Add(toogleSong);
            Add(previousSong);
            Add(nextSong);
        }
    }
}