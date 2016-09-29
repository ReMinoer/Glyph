using System.Collections;
using System.Collections.Generic;
using Fingear;

namespace Glyph.Input
{
    public abstract class ControlLayerBase : IControlLayer
    {
        private readonly List<IControl> _list = new List<IControl>();
        public bool Enabled { get; set; }
        public string DisplayName { get; }

        protected ControlLayerBase()
        {
            DisplayName = GetType().Name;
        }

        protected void Add(IControl control)
        {
            _list.Add(control);
        }

        public IEnumerator<IControl> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}