using System.Collections;
using System.Collections.Generic;
using Fingear;
using Glyph.Composition;
using Glyph.Injection;

namespace Glyph.Core.Inputs
{
    public class Controls : GlyphComponent, IEnableable, IControlLayer
    {
        private readonly ControlManager _controlManager;
        private readonly ControlLayer _controlLayer;
        public ICollection<object> Tags => _controlLayer.Tags;

        public bool Enabled
        {
            get => _controlLayer.Enabled;
            set => _controlLayer.Enabled = value;
        }

        public Controls(ControlManager controlManager, [GlyphInjectable(GlyphInjectableTargets.Parent)] IGlyphComponent parent = null)
        {
            _controlManager = controlManager;

            string name = parent?.Name != null ? parent.Name + " controls" : null;
            _controlLayer = new ControlLayer{ Name = name };
        }

        public override void Initialize()
        {
            _controlManager.Plan(_controlLayer);
        }

        public ControlManager.SchedulerController Plan()
        {
            return _controlManager.Plan(_controlLayer);
        }

        public override void Dispose()
        {
            _controlManager.Unplan(_controlLayer);
            base.Dispose();
        }

        public int Count => _controlLayer.Count;
        public void Register(IControl item) => _controlLayer.Register(item);
        public bool Unregister(IControl item) => _controlLayer.Unregister(item);
        public void Clear() => _controlLayer.Clear();
        public void ClearDisposed() => _controlLayer.ClearDisposed();
        public bool Contains(IControl item) => _controlLayer.Contains(item);
        public IEnumerator<IControl> GetEnumerator() => _controlLayer.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_controlLayer).GetEnumerator();
    }
}