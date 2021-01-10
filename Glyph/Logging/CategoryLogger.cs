using System;
using Glyph.Utils;
using Microsoft.Extensions.Logging;

namespace Glyph.Logging
{
    public class CategoryLogger : ILogger
    {
        private readonly string _category;
        public ILogger Logger { get; set; }

        public CategoryLogger(string category)
        {
            _category = category;
        }

        public bool IsEnabled(LogLevel logLevel) => Logger?.IsEnabled(logLevel) ?? false;
        public IDisposable BeginScope<TState>(TState state) => Logger?.BeginScope(state) ?? NullDisposable.Instance;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (Logger == null)
                return;

            using (Logger.Category(_category))
                Logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}