using System;
using Microsoft.Xna.Framework;

namespace Glyph.Transition
{
    public abstract class TransitionVector<T> : ITransition<T>
        where T : IEquatable<T>
    {
        private readonly bool[] _isAttributeInit = {false, false, false};
        private float _actualDuration;
        private T _actualEnd;
        private T _actualStart;
        private float _delay;
        private float _duration;
        private float _elapsedDelay;
        private float _elapsedTime;
        private T _end;
        private ITimingFunction _function;
        private bool _isInit;
        private bool _lastReverse;
        private T _lastSpeed;
        private T _lastUpdate;
        private float _reverseFactor;
        private T _start;
        private float _tempDelay;
        private float _tempDuration;
        private T _tempEnd;
        private ITimingFunction _tempFunction;
        private T _tempStart;
        private T _value;

        public T LastUpdate
        {
            get { return _lastUpdate; }
        }

        public T LastSpeed
        {
            get { return _lastSpeed; }
        }

        public ITimingFunction Function
        {
            get
            {
                if (_function != _tempFunction && (IsEnd || IsWaiting))
                    _function = _tempFunction;

                return _function;
            }
            set
            {
                _tempFunction = value;

                if (_function != _tempFunction && (IsEnd || IsWaiting))
                    _function = _tempFunction;
            }
        }

        public T Start
        {
            get
            {
                if (_start.Equals(_tempStart) || (!IsEnd && !IsWaiting))
                    return _start;

                _start = _tempStart;
                _actualStart = _tempStart;
                if (IsEnd)
                    Reset();

                return _start;
            }
            set
            {
                _tempStart = value;
                _isAttributeInit[0] = true;

                if (_start.Equals(_tempStart) || (!IsEnd && !IsWaiting))
                    return;

                _start = _tempStart;
                _actualStart = _tempStart;
                if (IsEnd)
                    Reset();
            }
        }

        public T End
        {
            get
            {
                if (_end.Equals(_tempEnd) || (!IsEnd && !IsWaiting))
                    return _end;

                _end = _tempEnd;
                _actualEnd = _tempEnd;
                if (IsEnd)
                    Reset();

                return _end;
            }
            set
            {
                _tempEnd = value;
                _isAttributeInit[1] = true;

                if (_end.Equals(_tempEnd) || (!IsEnd && !IsWaiting))
                    return;

                _end = _tempEnd;
                _actualEnd = _tempEnd;
                if (IsEnd)
                    Reset();
            }
        }

        public float Duration
        {
            get
            {
                if (!(Math.Abs(_duration - _tempDuration) > float.Epsilon) || (!IsEnd && !IsWaiting))
                    return _duration;

                _duration = _tempDuration;
                _actualDuration = _tempDuration;
                if (IsEnd)
                    Reset();
                return _duration;
            }
            set
            {
                _tempDuration = value;
                _isAttributeInit[2] = true;

                if (!(Math.Abs(_duration - _tempDuration) > float.Epsilon) || (!IsEnd && !IsWaiting))
                    return;

                _duration = _tempDuration;
                _actualDuration = _tempDuration;
                if (IsEnd)
                    Reset();
            }
        }

        public float MeanSpeed
        {
            set
            {
                if (!(_isAttributeInit[0] && _isAttributeInit[1]))
                    throw new InvalidOperationException("You need to initialize Start and End before define MeanSpeed.");

                Duration = MeanSpeedToDuration(value);
            }
        }

        public float Delay
        {
            get
            {
                if (!(Math.Abs(_delay - _tempDelay) > float.Epsilon) || (IsWaiting && _isInit))
                    return _delay;

                _delay = _tempDelay;
                _elapsedDelay = _delay;
                return _delay;
            }
            set
            {
                _tempDelay = value;

                if (!(Math.Abs(_delay - _tempDelay) > float.Epsilon) || (IsWaiting && _isInit))
                    return;

                _delay = _tempDelay;
                _elapsedDelay = _delay;
            }
        }

        public T Value
        {
            get { return _value; }
        }

        public float ElapsedTime
        {
            get { return _elapsedTime; }
            set
            {
                if (value < 0 || value > Duration)
                    throw new ArgumentException("Value must be between 0 and Duration");
                _elapsedTime = value;
            }
        }

        public bool IsEnd
        {
            get { return ElapsedTime >= _actualDuration; }
        }

        public bool IsWaiting
        {
            get { return _elapsedDelay < _delay || Math.Abs(ElapsedTime) < float.Epsilon; }
        }

        private bool IsAllAttributesInit
        {
            get { return _isAttributeInit[0] && _isAttributeInit[1] && _isAttributeInit[2]; }
        }

        protected TransitionVector(ITimingFunction f)
        {
            Function = f;
            _isInit = false;
        }

        protected TransitionVector()
            : this(new LinearFunction())
        {
        }

        protected TransitionVector(float p1X, float p1Y, float p2X, float p2Y)
            : this(new BezierFunction(p1X, p1Y, p2X, p2Y))
        {
        }

        protected TransitionVector(int steps, bool startInclude)
            : this(new StepsFunction(steps, startInclude))
        {
        }

        protected abstract T Add(T a, T b);
        protected abstract T Subtract(T a, T b);
        protected abstract T Scalar(T a, float b);
        protected abstract float Ratio(T a, T b);
        protected abstract T Normalize(T a);

        public T Update(GameTime gameTime, T start, T end, float duration, bool reverse = false, bool reset = false)
        {
            if (!Start.Equals(start) || !End.Equals(end) || !Duration.Equals(duration))
                Init(start, end, duration, reset);

            return Update(gameTime, reverse);
        }

        public T UpdateBySpeed(GameTime gameTime, T start, T end, float meanSpeed, bool reverse = false,
            bool reset = false)
        {
            if (!Start.Equals(start) || !End.Equals(end) || !Duration.Equals(MeanSpeedToDuration(meanSpeed)))
                InitBySpeed(start, end, meanSpeed, reset);

            return Update(gameTime, reverse);
        }

        static public implicit operator T(TransitionVector<T> x)
        {
            return x.Value;
        }

        public void ShiftValues(T newValue)
        {
            ShiftValuesRelative(Subtract(newValue, Value));
        }

        public void ShiftValuesRelative(T modif)
        {
            _value = Add(Value, modif);
            Start = Add(Start, modif);
            End = Add(End, modif);
            _actualStart = Add(_actualStart, modif);
            _actualEnd = Add(_actualEnd, modif);
        }

        public void Init(T start, T end, float duration, bool reset = false, bool fromEnd = false)
        {
            Start = start;
            End = end;
            Duration = duration;

            if (reset)
                Reset(fromEnd);

            _isInit = true;
        }

        public void InitBySpeed(T start, T end, float meanSpeed, bool reset = false, bool fromEnd = false)
        {
            Start = start;
            End = end;
            MeanSpeed = meanSpeed;

            if (reset)
                Reset(fromEnd);

            _isInit = true;
        }

        public void Reset(bool fromEnd = false)
        {
            _value = fromEnd ? CalculateValue(Duration) : CalculateValue(0);

            _elapsedTime = fromEnd ? Duration : 0;
            _elapsedDelay = fromEnd ? Delay : 0;

            _actualStart = Start;
            _actualEnd = End;
            _actualDuration = Duration;

            _lastUpdate = default(T);
            _lastSpeed = default(T);

            _lastReverse = false;
            _reverseFactor = 1f;
        }

        public T Update(GameTime gameTime, bool reverse)
        {
            if (!IsAllAttributesInit)
                throw new InvalidOperationException((!_isAttributeInit[0] ? "Start not initialized ! " : "")
                                                    + (!_isAttributeInit[1] ? "End not initialized ! " : "")
                                                    + (!_isAttributeInit[2]
                                                        ? "Duration or MeanSpeed not initialized ! "
                                                        : ""));

            if (!_isInit)
            {
                Reset();
                _isInit = true;
            }

            if (_lastReverse != reverse)
                Reverse(reverse, Value);

            if (Value.Equals(_actualEnd))
                _elapsedDelay = 0;

            if (_elapsedDelay < Delay && Value.Equals(_actualStart))
            {
                _elapsedDelay += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_elapsedDelay > Delay)
                    _elapsedTime += _elapsedDelay - Delay;
            }
            else
            {
                _elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (ElapsedTime > _actualDuration)
                    _elapsedTime = _actualDuration;
            }

            if (_actualDuration > 0)
            {
                T newValue = CalculateValue(ElapsedTime);
                _lastUpdate = Subtract(newValue, Value);
                _lastSpeed = Scalar(LastUpdate, (1 / (float)gameTime.ElapsedGameTime.TotalMilliseconds));
                _value = newValue;
            }
            else
            {
                _value = _actualEnd;
                _lastUpdate = default(T);
                _lastSpeed = default(T);
            }

            return Value;
        }

        public T Update(GameTime gameTime)
        {
            return Update(gameTime, _lastReverse);
        }

        public void ToggleReverse()
        {
            Reverse(!_lastReverse, Value);
        }

        public T ProvisionalValue(float milliseconds)
        {
            if (milliseconds < 0 || milliseconds > Duration)
                throw new ArgumentException("Parameter must be >= to 0 and <= to Duration !");

            return CalculateValue(milliseconds);
        }

        public T ProvisionalValueRelative(float milliseconds)
        {
            if (milliseconds < 0 || milliseconds > (Duration - ElapsedTime))
                throw new ArgumentException("Parameter must be >= to 0 and <= to (Duration - ElapsedTime) !");

            return CalculateValue(ElapsedTime + milliseconds);
        }

        private T CalculateValue(float time)
        {
            if (time < 0 || time > Duration)
                throw new ArgumentException("Parameter must be >= to 0 and <= to Duration !");

            return Add(_actualStart,
                Scalar(Subtract(_actualEnd, _actualStart), Function.GetValue(time / _actualDuration)));
        }

        private void Reverse(bool reverse, T actual)
        {
            _actualEnd = reverse ? Start : End;
            _actualStart = actual;

            _reverseFactor = ((_actualDuration > 0 ? Function.GetValue(ElapsedTime / _actualDuration) : 1)
                              * _reverseFactor) + (1 - _reverseFactor);
            _actualDuration = Duration * _reverseFactor;

            _elapsedTime = 0;
            _lastReverse = reverse;
        }

        private float MeanSpeedToDuration(float value)
        {
            T diff = Subtract(End, Start);
            return Ratio(diff, Scalar(Normalize(diff), value / 1000));
        }
    }
}