using System.Threading;
using System.Threading.Tasks;

namespace Glyph.Content
{
    public delegate T LoadDelegate<T>(string assetPath, CancellationToken cancellationToken);
    public delegate Task<T> LoadAsyncDelegate<T>(string assetPath, CancellationToken cancellationToken);
}