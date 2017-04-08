using Glyph.Space;

namespace Glyph.Scripting
{
    public class TriggerManager
    {
        private IWriteableSpace<Trigger> _space;

        public IWriteableSpace<Trigger> Space
        {
            get { return _space; }
            set
            {
                if (_space != null)
                {
                    foreach (Trigger trigger in _space)
                        value.Add(trigger);
                    _space.Clear();
                }

                _space = value;
            }
        }

        public TriggerManager()
        {
            Space = new Space<Trigger>(x => x.Shape);
        }

        internal bool Register(Trigger trigger)
        {
            return Space.Add(trigger);
        }

        internal bool Unregister(Trigger trigger)
        {
            return Space.Remove(trigger);
        }
    }
}