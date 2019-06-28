using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Glyph.Core;

namespace Glyph.Tools.Brushing.Controllers
{
    public class MultiBrushEditor<TCanvas> : GlyphObject, IIntegratedEditor<TCanvas>, ICollection<IBrushController<TCanvas>>
    {
        private TCanvas _canvas;
        private readonly List<IBrushController<TCanvas>> _brushControllers;

        public TCanvas Canvas
        {
            get => _canvas;
            set
            {
                _canvas = value;

                foreach (IBrushController<TCanvas> brushController in _brushControllers)
                    brushController.Canvas = _canvas;
            }
        }

        object IIntegratedEditor.EditedObject => Canvas;
        TCanvas IIntegratedEditor<TCanvas>.EditedObject => Canvas;

        public MultiBrushEditor(GlyphResolveContext context)
            : base(context)
        {
            _brushControllers = new List<IBrushController<TCanvas>>();

            foreach (IBrushController<TCanvas> brushController in _brushControllers)
            {
                brushController.ApplyStarted += BrushControllerOnApplyStarted;
                brushController.ApplyCancelled += BrushControllerOnApplyEnded;
                brushController.ApplyEnded += BrushControllerOnApplyEnded;
            }
        }

        private void BrushControllerOnApplyStarted(object sender, EventArgs e)
        {
            foreach (IBrushController<TCanvas> brushController in _brushControllers.Where(x => x != sender))
                brushController.Enabled = false;
        }

        private void BrushControllerOnApplyEnded(object sender, EventArgs e)
        {
            foreach (IBrushController<TCanvas> brushController in _brushControllers)
                brushController.Enabled = true;
        }

        public void AddBrush(IBrushController<TCanvas> item)
        {
            _brushControllers.Add(item);
            item.ApplyStarted += BrushControllerOnApplyStarted;
            item.ApplyCancelled += BrushControllerOnApplyEnded;
            item.ApplyEnded += BrushControllerOnApplyEnded;
        }

        public bool RemoveBrush(IBrushController<TCanvas> item)
        {
            if (!_brushControllers.Remove(item))
                return false;

            item.ApplyEnded -= BrushControllerOnApplyEnded;
            item.ApplyCancelled -= BrushControllerOnApplyEnded;
            item.ApplyStarted -= BrushControllerOnApplyStarted;
            return true;
        }

        public void ClearBrush()
        {
            foreach (IBrushController<TCanvas> brushController in _brushControllers)
            {
                brushController.ApplyEnded -= BrushControllerOnApplyEnded;
                brushController.ApplyCancelled -= BrushControllerOnApplyEnded;
                brushController.ApplyStarted -= BrushControllerOnApplyStarted;
            }

            _brushControllers.Clear();
        }

        public bool ContainsBrush(IBrushController<TCanvas> item) => _brushControllers.Contains(item);
        
        int ICollection<IBrushController<TCanvas>>.Count => _brushControllers.Count;
        bool ICollection<IBrushController<TCanvas>>.IsReadOnly => false;
        void ICollection<IBrushController<TCanvas>>.Add(IBrushController<TCanvas> item) => AddBrush(item);
        bool ICollection<IBrushController<TCanvas>>.Remove(IBrushController<TCanvas> item) => RemoveBrush(item);
        bool ICollection<IBrushController<TCanvas>>.Contains(IBrushController<TCanvas> item) => ContainsBrush(item);
        void ICollection<IBrushController<TCanvas>>.CopyTo(IBrushController<TCanvas>[] array, int arrayIndex) => _brushControllers.CopyTo(array, arrayIndex);
        IEnumerator<IBrushController<TCanvas>> IEnumerable<IBrushController<TCanvas>>.GetEnumerator() => _brushControllers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_brushControllers).GetEnumerator();

    }
}