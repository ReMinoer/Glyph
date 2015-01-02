using System.Collections.Generic;
using System.Diagnostics;

namespace Glyph.Tools
{
    static public class Chronometer
    {
        static public List<Stopwatch> Timer { get; set; }

        static public void Init(int nbTimers = 5)
        {
            Timer = new List<Stopwatch>();
            for (int i = 0; i < nbTimers; i++)
                Timer.Add(new Stopwatch());
        }
    }
}