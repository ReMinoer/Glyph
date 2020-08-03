using System.Threading;
using System.Threading.Tasks;

namespace Glyph.Content
{
    public delegate Task<T> LoadDelegate<T>(string assetPath, CancellationToken cancellationToken);
}