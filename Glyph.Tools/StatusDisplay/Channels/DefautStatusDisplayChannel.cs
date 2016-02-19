using System.Globalization;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.StatusDisplay.Channels
{
    public class DefautStatusDisplayChannel : StatusDisplayChannel
    {
        private readonly PerformanceViewer _performanceViewer;

        public DefautStatusDisplayChannel(PerformanceViewer performanceViewer)
        {
            _performanceViewer = performanceViewer;

            Text["fps"] = new StatusDisplayText("FPS");
            Text["update"] = new StatusDisplayText("Update (ms)");
            Text["draw"] = new StatusDisplayText("Draw (ms)");
            Text["ram"] = new StatusDisplayText("Mémoire allouée (Mo)");
            Text["ramMax"] = new StatusDisplayText("Mémoire max (Mo)");
        }

        protected override void UpdateValues(GameTime gameTime)
        {
            Text["fps"].Text = _performanceViewer.Fps.ToString(CultureInfo.InvariantCulture);
            Text["fps"].Color = _performanceViewer.Fps < 59 ? Color.Red : Color.White;

            Text["update"].Text = _performanceViewer.UpdatePeak + " (" + _performanceViewer.UpdateMean + ")";
            Text["draw"].Text = _performanceViewer.DrawPeak + " (" + _performanceViewer.DrawMean + ")";

            Text["ram"].Text = (_performanceViewer.Memory / 1000) + (_performanceViewer.IsIncrease ? " ↑" : " ↓");
            Text["ramMax"].Text = (_performanceViewer.MemoryMax / 1000) + (_performanceViewer.IsIncreaseMax ? " ↑" : "");
        }
    }
}