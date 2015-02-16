using System;
using Diese.Debug;

namespace Glyph.Debug
{
    static public class LogTagGlyph
    {
        static public LogTag System = new LogTag(ConsoleColor.Green, "###");
        static public LogTag Warning = new LogTag(ConsoleColor.Yellow, "***");
        static public LogTag Error = new LogTag(ConsoleColor.Red, "!!!");
        static public LogTag LoadedContent = new LogTag(ConsoleColor.Cyan, ">>>");
        static public LogTag GameEvent = new LogTag(ConsoleColor.Magenta, "#");
        static public LogTag Script = new LogTag(ConsoleColor.Gray, ">");
    }
}