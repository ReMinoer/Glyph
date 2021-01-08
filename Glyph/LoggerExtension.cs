using Microsoft.Extensions.Logging;

namespace Glyph
{
    static public class LoggerExtension
    {
        static public void Trace(this ILogger logger, string message) => Log(logger, LogLevel.Trace, message);
        static public void Debug(this ILogger logger, string message) => Log(logger, LogLevel.Debug, message);
        static public void Info(this ILogger logger, string message) => Log(logger, LogLevel.Information, message);
        static public void Warning(this ILogger logger, string message) => Log(logger, LogLevel.Warning, message);
        static public void Error(this ILogger logger, string message) => Log(logger, LogLevel.Error, message);
        static public void Critical(this ILogger logger, string message) => Log(logger, LogLevel.Critical, message);

        static private void Log(ILogger logger, LogLevel level, string message)
        {
            logger.Log(level, message);
        }
    }
}