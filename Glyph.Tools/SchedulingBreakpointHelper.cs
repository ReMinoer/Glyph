using System.Diagnostics;
using Diese;
using Glyph.Composition.Delegates;
using NLog;

namespace Glyph.Tools
{
    static public class SchedulingBreakpointHelper
    {
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static public InitializeDelegate Initialize(Predicate predicate = null)
        {
            return () =>
            {
                if (predicate != null && !predicate())
                    return;

                Logger.Debug("Initialize breakpoint");
                Debugger.Break();
            };
        }

        static public LoadContentDelegate LoadContent(Predicate predicate = null)
        {
            return async x =>
            {
                if (predicate != null && !predicate())
                    return;

                Logger.Debug("LoadContent breakpoint");
                Debugger.Break();
            };
        }

        static public UpdateDelegate Update(Predicate predicate = null)
        {
            return x =>
            {
                if (predicate != null && !predicate())
                    return;

                Logger.Debug("Update breakpoint");
                Debugger.Break();
            };
        }

        static public DrawDelegate Draw(Predicate predicate = null)
        {
            return x =>
            {
                if (predicate != null && !predicate())
                    return;

                Logger.Debug("Draw breakpoint");
                Debugger.Break();
            };
        }
    }
}