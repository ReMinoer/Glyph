using System;

namespace Glyph.Tools.Brushing.Controllers
{
    public interface IBrushController<TCanvas>
    {
        bool Enabled { get; set; }
        TCanvas Canvas { get; set; }
        bool ApplyingBrush { get; }

        event EventHandler ApplyStarted;
        event EventHandler ApplyCancelled;
        event EventHandler ApplyEnded;
    }

    public interface IBrushController<TCanvas, TBrushArgs, TPaint> : IBrushController<TCanvas>
        where TPaint : IPaint
    {
        IBrush<TCanvas, TBrushArgs, TPaint> Brush { get; set; }
        TPaint Paint { get; set; }
    }
}