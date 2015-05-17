using Glyph.Audio;
using Glyph.Input;
using Glyph.Input.StandardInputs;
using Microsoft.Xna.Framework.Media;

namespace Glyph.Tools
{
    static public class SongPlayer
    {
        static public void HandleInput(InputManager input)
        {
            if (input[DeveloperInputs.ToogleSong])
                switch (MediaPlayer.State)
                {
                    case MediaState.Playing:
                        MediaPlayer.Pause();
                        break;
                    case MediaState.Paused:
                        MediaPlayer.Resume();
                        break;
                }

            if (input[DeveloperInputs.PreviousSong])
                AudioManager.Previous();

            if (input[DeveloperInputs.NextSong])
                AudioManager.Next();
        }
    }
}