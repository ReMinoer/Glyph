using Microsoft.Extensions.Logging;

namespace Glyph.Logging
{
    public interface ILogging
    {
        ILogger Logger { set; }
    }
}