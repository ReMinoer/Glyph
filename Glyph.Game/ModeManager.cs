using NLog;

namespace Glyph.Game
{
    public class ModeManager<TState>
    {
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool _isChange;
        private TState _state;

        public TState State
        {
            get { return _state; }
            set
            {
                if (_state.Equals(value))
                    return;

                _state = value;
                _isChange = true;
                Logger.Info("Current mode : " + State);
            }
        }

        public void Initialize(TState state)
        {
            _state = state;
            Logger.Info("Current mode : " + State);
        }

        public bool HasChange()
        {
            if (!_isChange)
                return false;

            _isChange = false;
            return true;
        }
    }
}