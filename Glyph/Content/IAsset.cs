using System;
using System.Threading;
using System.Threading.Tasks;
using Glyph.Threading;

namespace Glyph.Content
{
    public interface IAsset
    {
        string AssetPath { get; }
        void Handle();
        Task<bool> ReleaseAsync();
        event EventHandler ContentChanged;
        event EventHandler FullyReleasing;
        event EventHandler FullyReleased;
    }

    public interface IAsset<out T> : IAsset
    {
        ITask<T> GetContentAsync(CancellationToken cancellationToken = default);
    }
}