using Glyph.Input;
using Microsoft.Xna.Framework.Media;

namespace Glyph.Tools
{
    static public class SongPlayer
    {
        static public void HandleInput(InputManager input)
        {
            if (input.IsActionDownNow(DeveloperActions.ToogleSong))
                switch (MediaPlayer.State)
                {
                    case MediaState.Playing:
                        MediaPlayer.Pause();
                        break;
                    case MediaState.Paused:
                        MediaPlayer.Resume();
                        break;
                }

            if (input.IsActionDownNow(DeveloperActions.PreviousSong))
                AudioManager.Previous();

            if (input.IsActionDownNow(DeveloperActions.NextSong))
                AudioManager.Next();
        }
    }
}