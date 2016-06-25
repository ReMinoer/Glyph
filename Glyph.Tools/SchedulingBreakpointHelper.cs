using System.Diagnostics;
using NLog;

namespace Glyph.Tools
{
    static public class SchedulingBreakpointHelper
    {
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static public void Initialize()
        {
            Logger.Debug("Initialize breakpoint");
            Debugger.Break();
        }

        static public void LoadContent(ContentLibrary contentLibrary)
        {
            Logger.Debug("LoadContent breakpoint");
            Debugger.Break();
        }

        static public void Update(ElapsedTime elapsedTime)
        {
            Logger.Debug("Update breakpoint");
            Debugger.Break();
        }

        static public void Draw(IDrawer drawer)
        {
            Logger.Debug("Draw breakpoint");
            Debugger.Break();
        }
    }
}