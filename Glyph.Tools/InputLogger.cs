using System;
using System.Linq;
using Fingear;
using Glyph.Composition;
using Glyph.Input;
using NLog;

namespace Glyph.Tools
{
    public class InputLogger : GlyphComponent, IUpdate, IEnableable
    {
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ControlManager _controlManager;
        public bool Enabled { get; set; }
        public Predicate<IControlLayer> LayerFilter { get; set; }
        public Predicate<IInput> InputFilter { get; set; }

        public InputLogger(ControlManager controlManager)
        {
            Enabled = true;
            _controlManager = controlManager;
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;
            
            foreach (IControlLayer layer in _controlManager.Layers.Where(x => x.Enabled))
            {
                if (LayerFilter != null && !LayerFilter(layer))
                    continue;

                foreach (IInput input in layer.SelectMany(x => x.Inputs))
                {
                    if (InputFilter != null && !InputFilter(input))
                        continue;

                    InputActivity inputActivity = input.Activity;
                    if (!inputActivity.IsChanged())
                        continue;

                    string activityName;
                    switch (inputActivity)
                    {
                        case InputActivity.Triggered:
                            activityName = "triggered";
                            break;
                        case InputActivity.Released:
                            activityName = "released";
                            break;
                        default: throw new NotSupportedException();
                    }

                    Logger.Debug($"({input.Source.DisplayName}) {input.DisplayName} is {activityName}");
                }
            }
        }
    }
}