using System.Globalization;
using Microsoft.Xna.Framework;

namespace Glyph.Tools
{
    internal class DefautStatusDisplayChannel : StatusDisplayChannel
    {
        public DefautStatusDisplayChannel()
        {
            Text["fps"] = new StatusDisplayText("FPS");
            Text["update"] = new StatusDisplayText("Update (ms)");
            Text["draw"] = new StatusDisplayText("Draw (ms)");
            Text["ram"] = new StatusDisplayText("Mémoire allouée (Mo)");
            Text["ramMax"] = new StatusDisplayText("Mémoire max (Mo)");
        }

        public override void UpdateValues(GameTime gameTime, Game game)
        {
            Text["fps"].Text = game.PerformanceViewer.Fps.ToString(CultureInfo.InvariantCulture);
            Text["fps"].Color = game.PerformanceViewer.Fps < 59 ? Color.Red : Color.White;

            Text["update"].Text = game.PerformanceViewer.UpdatePeak + " (" + game.PerformanceViewer.UpdateMean + ")";
            Text["draw"].Text = game.PerformanceViewer.DrawPeak + " (" + game.PerformanceViewer.DrawMean + ")";

            Text["ram"].Text = (game.PerformanceViewer.Memory / 1000)
                               + (game.PerformanceViewer.IsIncrease ? " ↑" : " ↓");
            Text["ramMax"].Text = (game.PerformanceViewer.MemoryMax / 1000)
                                  + (game.PerformanceViewer.IsIncreaseMax ? " ↑" : "");
        }
    }
}