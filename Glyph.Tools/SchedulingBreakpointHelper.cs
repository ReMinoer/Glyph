using System.Diagnostics;
using Diese;
using Glyph.Scheduling;

namespace Glyph.Tools
{
    static public class SchedulingBreakpointHelper
    {
        static public InitializeDelegate Initialize(Predicate predicate = null)
        {
            return () =>
            {
                if (predicate == null || predicate())
                    Debugger.Break();
            };
        }

        static public LoadContentDelegate LoadContent(Predicate predicate = null)
        {
            return async x =>
            {
                if (predicate == null || predicate())
                    Debugger.Break();
            };
        }

        static public UpdateDelegate Update(Predicate predicate = null)
        {
            return x =>
            {
                if (predicate == null || predicate())
                    Debugger.Break();
            };
        }

        static public DrawDelegate Draw(Predicate predicate = null)
        {
            return x =>
            {
                if (predicate == null || predicate())
                    Debugger.Break();
            };
        }
    }
}