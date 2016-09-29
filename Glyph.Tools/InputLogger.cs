using System;
using System.Linq;
using Diese.Injection;
using Fingear;
using Glyph.Composition;
using NLog;

namespace Glyph.Tools
{
    public class InputLogger : GlyphComponent, IUpdate, IEnableable
    {
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private IInput[] _inputs;
        public bool Enabled { get; set; }
        public Predicate<IInput> Filter { get; set; }

        [Injectable]
        public InputSystem InputSystem
        {
            set { _inputs = value.SelectMany(source => source.GetAllInputs()).ToArray(); }
        }

        public InputLogger()
        {
            Enabled = true;
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            foreach (IInput input in _inputs)
            {
                input.Update();

                if (Filter != null && !Filter(input))
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