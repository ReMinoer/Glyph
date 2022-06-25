using System;
using Glyph.Core;
using Glyph.Tools.UndoRedo;
using Niddle.Attributes;

namespace Glyph.Tools.Brushing.Controllers.Base
{
    public abstract class BrushControllerBase<TCanvas, TBrush, TBrushArgs, TPaint> : GlyphObject, IBrushController<TCanvas, TBrush, TBrushArgs, TPaint>, IIntegratedEditor<TCanvas>
        where TBrush : IBrush<TCanvas, TBrushArgs, TPaint>
        where TPaint : IPaint
    {
        private TCanvas _canvas;
        public TCanvas Canvas
        {
            get => _canvas;
            set
            {
                if (ApplyingBrush)
                    Cancel(default(TBrushArgs));

                _canvas = value;
            }
        }

        private TBrush _brush;
        public virtual TBrush Brush
        {
            get => _brush;
            set
            {
                if (ApplyingBrush)
                    Cancel(default(TBrushArgs));

                _brush = value;
            }
        }

        private TPaint _paint;
        public TPaint Paint
        {
            get => _paint;
            set
            {
                if (ApplyingBrush)
                    Cancel(default(TBrushArgs));

                _paint = value;
            }
        }

        public bool ApplyingBrush { get; private set; }

        [Resolvable]
        public IUndoRedoStack UndoRedoStack { get; set; }

        object IIntegratedEditor.EditedObject => Canvas;
        TCanvas IIntegratedEditor<TCanvas>.EditedObject => Canvas;

        public event EventHandler ApplyStarted;
        public event EventHandler ApplyCancelled;
        public event EventHandler ApplyEnded;

        protected BrushControllerBase(GlyphResolveContext context)
            : base(context)
        {
            Schedulers.Update.Plan(UpdateLocal);
        }

        private void UpdateLocal(ElapsedTime elapsedTime)
        {
            if (Canvas == null)
                return;

            TBrushArgs args = GetBrushArgs(Canvas);

            Brush?.Update(Canvas, args, Paint);

            if (!ApplyingBrush)
            {
                if (RequestingApplyStart && Canvas != null && Brush != null && Paint != null)
                {
                    if (Brush.CanStartApply(Canvas, args, Paint))
                    {
                        ApplyingBrush = true;
                        Brush.StartApply(Canvas, args, Paint);

                        OnApplyStarted(args);
                        ApplyStarted?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        Brush.OnInvalidStart(Canvas, args, Paint);
                        OnStartInvalided(args);
                    }
                }
            }
            else
            {
                if (RequestingApplyEnd)
                {
                    ApplyingBrush = false;

                    if (Brush.CanEndApply(Canvas, args, Paint))
                    {
                        Brush.EndApply(Canvas, args, Paint, UndoRedoStack);

                        OnApplyEnded(args);
                        ApplyEnded?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        Brush.OnInvalidEnd(Canvas, args, Paint);
                        OnEndInvalided(args);
                    }
                }
            }

            if (ApplyingBrush)
            {
                if (RequestingCancellation)
                {
                    Cancel(args);
                }
                else
                {
                    Brush.UpdateApply(Canvas, args, Paint);
                    OnApplyUpdated(args);
                }
            }
        }

        private void Cancel(TBrushArgs args)
        {
            ApplyingBrush = false;
            Brush.OnCancellation(Canvas, args, Paint);

            OnCancelled(args);
            ApplyCancelled?.Invoke(this, EventArgs.Empty);
        }

        protected abstract bool RequestingApplyStart { get; }
        protected abstract bool RequestingApplyEnd { get; }
        protected abstract bool RequestingCancellation { get; }

        protected virtual void OnApplyStarted(TBrushArgs args) {}
        protected virtual void OnStartInvalided(TBrushArgs args) {}
        protected virtual void OnApplyEnded(TBrushArgs args) {}
        protected virtual void OnEndInvalided(TBrushArgs args) {}
        protected virtual void OnCancelled(TBrushArgs args) {}
        protected virtual void OnApplyUpdated(TBrushArgs args) { }

        protected abstract TBrushArgs GetBrushArgs(TCanvas canvas);
    }
}