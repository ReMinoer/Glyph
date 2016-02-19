using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Glyph.Tools
{
    public class PerformanceViewer
    {
        private const int RefreshIntervalle = 1000;
        private readonly Stopwatch _drawTimer = new Stopwatch();
        private readonly Period _refreshPeriod = new Period(RefreshIntervalle);
        private readonly Stopwatch _updateTimer = new Stopwatch();
        private float _drawCompteur;
        private float _drawMax;
        private float _drawTotals;
        private float _lastMemory;
        private float _updateCompteur;
        private float _updateMax;
        private float _updateTotals;
        public string ProcessName { get; private set; }
        public float Memory { get; private set; }
        public float MemoryMax { get; private set; }
        public float Fps { get; private set; }
        public float UpdatePeak { get; private set; }
        public float DrawPeak { get; private set; }
        public float UpdateMean { get; private set; }
        public float DrawMean { get; private set; }

        public bool IsIncrease
        {
            get { return Memory > _lastMemory; }
        }

        public bool IsIncreaseMax
        {
            get { return System.Math.Abs(Memory - MemoryMax) < float.Epsilon; }
        }

        public PerformanceViewer()
            : this(Process.GetCurrentProcess().ProcessName)
        {
        }

        public PerformanceViewer(string process)
        {
            ProcessName = process;
        }

        public void Update(GameTime gameTime)
        {
            if (!_refreshPeriod.Update(gameTime))
                return;

            Fps = _drawCompteur * (1 + (_refreshPeriod.ElapsedTime / RefreshIntervalle));

            _lastMemory = Memory;
            Memory = Process.GetProcessesByName(ProcessName)[0].PrivateMemorySize64 / 1024f;
            if (Memory > MemoryMax)
                MemoryMax = Memory;

            UpdatePeak = _updateMax;
            DrawPeak = _drawMax;

            UpdateMean = _updateTotals / _updateCompteur;
            DrawMean = _drawTotals / _drawCompteur;

            _updateCompteur = 0;
            _drawCompteur = 0;
            _updateMax = 0;
            _drawMax = 0;
            _updateTotals = 0;
            _drawTotals = 0;
        }

        public void UpdateCall()
        {
            _updateCompteur++;

            _updateTimer.Reset();
            _updateTimer.Start();
        }

        public void UpdateEnd()
        {
            _updateTimer.Stop();

            _updateTotals += (float)_updateTimer.Elapsed.TotalMilliseconds;

            if (_updateTimer.Elapsed.TotalMilliseconds > _updateMax)
                _updateMax = (float)_updateTimer.Elapsed.TotalMilliseconds;
        }

        public void DrawCall()
        {
            _drawCompteur++;

            _drawTimer.Reset();
            _drawTimer.Start();
        }

        public void DrawEnd()
        {
            _drawTimer.Stop();

            _drawTotals += (float)_drawTimer.Elapsed.TotalMilliseconds;

            if (_drawTimer.Elapsed.TotalMilliseconds > _drawMax)
                _drawMax = (float)_drawTimer.Elapsed.TotalMilliseconds;
        }
    }
}