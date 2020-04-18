using System;
using System.Collections.Generic;
using System.Linq;
using Fingear;
using Fingear.Controls;
using Fingear.Interactives;
using Glyph.Composition;
using NLog;
using Stave;

namespace Glyph.Tools
{
    public class ControlLogger : GlyphComponent, IUpdate
    {
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly InteractionManager _interactionManager;
        private readonly HashSet<IControl> _activateControls;
        public Predicate<IInteractive> InteractiveFilter { get; set; }
        public Predicate<IControl> ControlFilter { get; set; }

        public ControlLogger(InteractionManager interactionManager)
        {
            _interactionManager = interactionManager;
            _activateControls = new HashSet<IControl>();
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            foreach (IInteractive interactive in _interactionManager.Root.ChildrenQueue().Where(x => x.Enabled))
            {
                if (InteractiveFilter != null && !InteractiveFilter(interactive))
                    continue;

                foreach (IControl control in interactive.Controls)
                {
                    if (ControlFilter != null && !ControlFilter(control))
                        continue;

                    if (!control.IsActive)
                    {
                        _activateControls.Remove(control);
                        continue;
                    }

                    if (!_activateControls.Add(control))
                        continue;
                
                    Logger.Debug($"({interactive.Name}) {control.Name} is active");
                }
            }
        }
    }
}