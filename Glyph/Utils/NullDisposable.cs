using System;

namespace Glyph.Utils
{
    public class NullDisposable : IDisposable
    {
        static private NullDisposable _instance;
        static public NullDisposable Instance => _instance ?? (_instance = new NullDisposable());

        public void Dispose() { }
    }
}