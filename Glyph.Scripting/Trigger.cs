using System;

namespace Glyph.Scripting
{
    public class Trigger
    {
        private bool _enable;
        public string Name { get; set; }
        public bool UniqueUse { get; set; }

        public bool Enable
        {
            get { return _enable; }
            set
            {
                if (_enable == value)
                    return;
                if (_enable && UniqueUse)
                    return;

                _enable = value;

                if (_enable && Enabled != null)
                    Enabled.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler Enabled;

        public Trigger(string name, bool uniqueUse)
        {
            Name = name;
            UniqueUse = uniqueUse;

            _enable = false;
        }

        public override string ToString()
        {
            return string.Format("{0} : {1}", Name, Enable);
        }
    }
}