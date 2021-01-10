using System;
using System.Collections.Generic;
using Diese;
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

        static public void Trace<TCategory>(this ILogger logger, string message) => Log<TCategory>(logger, LogLevel.Trace, message);
        static public void Debug<TCategory>(this ILogger logger, string message) => Log<TCategory>(logger, LogLevel.Debug, message);
        static public void Info<TCategory>(this ILogger logger, string message) => Log<TCategory>(logger, LogLevel.Information, message);
        static public void Warning<TCategory>(this ILogger logger, string message) => Log<TCategory>(logger, LogLevel.Warning, message);
        static public void Error<TCategory>(this ILogger logger, string message) => Log<TCategory>(logger, LogLevel.Error, message);
        static public void Critical<TCategory>(this ILogger logger, string message) => Log<TCategory>(logger, LogLevel.Critical, message);

        static private void Log<TCategory>(ILogger logger, LogLevel level, string message)
        {
            using (logger.Category<TCategory>())
                logger.Log(level, message);
        }

        static public IDisposable Category<TCategory>(this ILogger logger) => logger.Category(typeof(TCategory));
        static public IDisposable Category(this ILogger logger, Type categoryType) => logger.Category(categoryType.GetDisplayName());
        static public IDisposable Category(this ILogger logger, string category)
        {
            return logger.BeginScope(new Dictionary<string, object> { ["Category"] = category });
        }
    }
}