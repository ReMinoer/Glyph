using System.Globalization;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.StatusDisplayChannels
{
    public class ChronometerStatusDisplayChannel : StatusDisplayChannel
    {
        public ChronometerStatusDisplayChannel()
        {
            OriginRight = true;
            OriginBottom = true;

            for (var i = 0; i < Chronometer.Timer.Count; i++)
                Text["chrono" + (i + 1)] = new StatusDisplayText("Chrono " + (i + 1));
        }

        protected override void UpdateValues(GameTime gameTime)
        {
            for (var i = 0; i < Chronometer.Timer.Count; i++)
                Text["chrono" + (i + 1)].Text =
                    Chronometer.Timer[i].Elapsed.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
        }
    }
}