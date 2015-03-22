using Microsoft.Xna.Framework;

namespace Glyph.Transition
{
    public class TransitionColor : ITransition<Color>
    {
        private readonly TransitionFloat _transitionFloat;
        private Color _value;
        public Color Start { get; set; }
        public Color End { get; set; }

        public ITimingFunction Function
        {
            get { return _transitionFloat.Function; }
            set { _transitionFloat.Function = value; }
        }

        public float Duration
        {
            get { return _transitionFloat.Duration; }
            set { _transitionFloat.Duration = value; }
        }

        public float MeanSpeed
        {
            set { _transitionFloat.MeanSpeed = value; }
        }

        public float Delay
        {
            get { return _transitionFloat.Delay; }
            set { _transitionFloat.Delay = value; }
        }

        public Color Value
        {
            get { return _value; }
        }

        public float ElapsedTime
        {
            get { return _transitionFloat.ElapsedTime; }
        }

        public bool IsEnd
        {
            get { return _transitionFloat.IsEnd; }
        }

        public bool IsWaiting
        {
            get { return _transitionFloat.IsWaiting; }
        }

        public TransitionColor(ITimingFunction f)
        {
            _transitionFloat = new TransitionFloat(f) {Start = 0, End = 1};
        }

        public TransitionColor()
            : this(new LinearFunction())
        {
        }

        public TransitionColor(float p1X, float p1Y, float p2X, float p2Y)
            : this(new BezierFunction(p1X, p1Y, p2X, p2Y))
        {
        }

        public TransitionColor(int steps, bool startInclude)
            : this(new StepsFunction(steps, startInclude))
        {
        }

        static public implicit operator Color(TransitionColor x)
        {
            return x.Value;
        }

        public void Init(Color start, Color end, float duration, bool reset = false, bool fromEnd = false)
        {
            Start = start;
            End = end;
            _transitionFloat.Init(0, 1, duration, reset, fromEnd);
        }

        public void InitBySpeed(Color start, Color end, float meanSpeed, bool reset = false, bool fromEnd = false)
        {
            Start = start;
            End = end;
            _transitionFloat.InitBySpeed(0, 1, meanSpeed, reset, fromEnd);
        }

        public void Reset(bool fromEnd = false)
        {
            _transitionFloat.Reset(fromEnd);
        }

        public Color Update(GameTime gameTime)
        {
            _transitionFloat.Update(gameTime);
            _value = Color.Lerp(Start, End, _transitionFloat.Value);
            return Value;
        }

        public Color Update(GameTime gameTime, bool reverse)
        {
            _transitionFloat.Update(gameTime, reverse);
            _value = Color.Lerp(Start, End, _transitionFloat.Value);
            return Value;
        }

        public void ToggleReverse()
        {
            _transitionFloat.ToggleReverse();
        }

        public Color ProvisionalValue(float milliseconds)
        {
            return Color.Lerp(Start, End, _transitionFloat.ProvisionalValue(milliseconds));
        }

        public Color ProvisionalValueRelative(float milliseconds)
        {
            return Color.Lerp(Start, End, _transitionFloat.ProvisionalValueRelative(milliseconds));
        }
    }
}