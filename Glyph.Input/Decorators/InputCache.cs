namespace Glyph.Input.Decorators
{
    public class InputCache<TValue> : InputDecorator<TValue, IInputHandler<TValue>>
    {
        private readonly Period _resetPeriod = new Period();
        private TValue _cache;

        public int ResetTimeout
        {
            get { return _resetPeriod.Interval; }
            set { _resetPeriod.Interval = value; }
        }

        public override bool IsActivated
        {
            get { return !_resetPeriod.IsEnd; }
        }

        public override TValue Value
        {
            get { return IsActivated ? _cache : default(TValue); }
        }

        public InputCache()
            : this("", null, 0)
        {
        }

        public InputCache(string name, IInputHandler<TValue> component, int resetTimeout)
            : base(name, component)
        {
            ResetTimeout = resetTimeout;
            _resetPeriod.InitFromEnd();
        }

        public override void Update(InputStates inputStates)
        {
            base.Update(inputStates);

            if (!IsActivated && Component.Value.Equals(default(TValue)))
                return;

            if (!_cache.Equals(Component.Value))
            {
                _resetPeriod.Init();
                _cache = Component.Value;
            }
            else
            {
                _resetPeriod.Update(ElapsedTime.Instance);
            }
        }
    }
}