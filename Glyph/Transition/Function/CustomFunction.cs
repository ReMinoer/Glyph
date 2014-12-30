using System;
using System.Collections.Generic;
using System.Linq;
using Glyph.Transition;
using Microsoft.Xna.Framework;

namespace Glyph
{
    // WATCH : CustomFunction pas testé
    public class CustomFunction : ITimingFunction
    {
        public Dictionary<Vector2, ITimingFunction> KeyPoints { get; private set; }

        public CustomFunction(ITimingFunction f)
        {
            KeyPoints = new Dictionary<Vector2, ITimingFunction> {{Vector2.Zero, f}};
        }

        public float GetValue(float t)
        {
            for (int i = 0; i < KeyPoints.Count; i++)
                if (t >= KeyPoints.ElementAt(i).Key.X)
                {
                    float x = KeyPoints.ElementAt(i).Key.X;
                    float y = KeyPoints.ElementAt(i).Key.Y;
                    ITimingFunction function = KeyPoints.ElementAt(i).Value;

                    float intervalX = (i + 1 < KeyPoints.Count) ? KeyPoints.ElementAt(i + 1).Key.X - x : 1f - x;
                    float intervalY = (i + 1 < KeyPoints.Count) ? KeyPoints.ElementAt(i + 1).Key.Y - y : 1f - y;

                    return (function.GetValue((t - x) / intervalX) * intervalY) + y;
                }
            return 1f;
        }

        public void AddKeyPoint(float x, float y, ITimingFunction f)
        {
            if (x < 0 || x > 1)
                throw new ArgumentException("0f <= x <= 1f");
            if (y < 0 || y > 1)
                throw new ArgumentException("0f <= y <= 1f");

            KeyPoints.Add(new Vector2(x, y), f);
            KeyPoints = KeyPoints.OrderBy(v => v.Key.X).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public void ClearKeyPoints(ITimingFunction f)
        {
            KeyPoints.Clear();
            KeyPoints.Add(new Vector2(0, 0), f);
        }
    }
}