using System.Globalization;
using Microsoft.Xna.Framework;

namespace Glyph.Tools
{
    public class ChronometerStatusDisplayChannel : StatusDisplayChannel
    {
        public ChronometerStatusDisplayChannel()
        {
            OriginRight = true;
            OriginBottom = true;

            for (int i = 0; i < Chronometer.Timer.Count; i++)
                Text["chrono" + (i + 1)] = new StatusDisplayText("Chrono " + (i + 1));
        }

        public override void UpdateValues(GameTime gameTime, Game game)
        {
            for (int i = 0; i < Chronometer.Timer.Count; i++)
                Text["chrono" + (i + 1)].Text =
                    Chronometer.Timer[i].Elapsed.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
        }
    }
}