﻿using System;

namespace Glyph.Scripting.Triggers
{
    public class TriggerVar : ITrigger
    {
        private bool _active;
        public string Name { get; set; }
        public bool SingleUse { get; private set; }

        public bool Active
        {
            get { return _active; }
            set
            {
                if (_active == value)
                    return;
                if (_active && SingleUse)
                    return;

                _active = value;

                if (_active && Triggered != null)
                    Triggered.Invoke(this);
            }
        }

        public event Action<ITrigger> Triggered;

        public TriggerVar(bool singleUse = false)
        {
            SingleUse = singleUse;
        }

        internal void SetActive(bool active)
        {
            _active = active;
        }

        public override string ToString()
        {
            return string.Format("{0} : {1}", Name, Active);
        }
    }
}