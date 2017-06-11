using System;
using System.Collections.Generic;
using System.Linq;
using Fingear;
using Glyph.Composition;
using NLog;

namespace Glyph.Tools
{
    public class ControlLogger : GlyphComponent, IUpdate, IEnableable
    {
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ControlManager _controlManager;
        private readonly HashSet<IControl> _activateControls;
        public bool Enabled { get; set; }
        public Predicate<IControlLayer> LayerFilter { get; set; }
        public Predicate<IControl> ControlFilter { get; set; }

        public ControlLogger(ControlManager controlManager)
        {
            Enabled = true;
            _controlManager = controlManager;
            _activateControls = new HashSet<IControl>();
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            foreach (IControlLayer layer in _controlManager.Layers.Where(x => x.Enabled))
            {
                if (LayerFilter != null && !LayerFilter(layer))
                    continue;

                foreach (IControl control in layer)
                {
                    if (ControlFilter != null && !ControlFilter(control))
                        continue;

                    if (!control.IsActive())
                    {
                        _activateControls.Remove(control);
                        continue;
                    }

                    if (!_activateControls.Add(control))
                        continue;
                
                    Logger.Debug($"({layer.DisplayName}) {control.Name} is active");
                }
            }
        }
    }
}