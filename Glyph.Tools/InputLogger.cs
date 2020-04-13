using System;
using System.Linq;
using Fingear.Inputs;
using Fingear.MonoGame;
using Glyph.Composition;
using NLog;

namespace Glyph.Tools
{
    public class InputLogger : GlyphComponent, IUpdate
    {
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public Predicate<IInput> InputFilter { get; set; }

        public InputLogger()
        {
            Enabled = true;
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;
            
            foreach (IInput input in InputSystem.Instance.Sources.SelectMany(x => x.InstantiatedInputs))
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