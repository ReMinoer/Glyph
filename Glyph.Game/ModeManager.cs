﻿using Diese.Debug;
using Glyph.Debug;

namespace Glyph.Game
{
    public class ModeManager<TState>
    {
        public TState State
        {
            get { return _state; }
            set
            {
                if (_state.Equals(value))
                    return;

                _state = value;
                _isChange = true;
                Log.Message("Current mode : " + State, LogTagGlyph.GameEvent);
            }
        }
        private bool _isChange;
        private TState _state;

        public void Initialize(TState state)
        {
            _state = state;
            Log.Message("Current mode : " + State, LogTagGlyph.GameEvent);
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