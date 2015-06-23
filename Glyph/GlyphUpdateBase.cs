using System;

namespace Glyph
{
    public abstract class GlyphUpdateBase : GlyphComponent, IUpdate
    {
        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled == value)
                    return;

                if (EnabledChanged != null)
                    EnabledChanged(this, EventArgs.Empty);

                _enabled = value;
            }
        }

        public event EventHandler EnabledChanged;

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            UpdateLocal(elapsedTime);
        }

        protected abstract void UpdateLocal(ElapsedTime elapsedTime);
    }
}