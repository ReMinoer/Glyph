using System.Threading;
using System.Threading.Tasks;
using Glyph.Composition;
using Glyph.Content;
using Microsoft.Xna.Framework.Media;

namespace Glyph.Audio
{
    public class SongPlayer : GlyphComponent, ILoadContent, IUpdate
    {
        static private readonly AssetAsyncLoader<Song> AssetAsyncLoader = new AssetAsyncLoader<Song>();
        static private Song _song;
        static private string _assetPath;

        private readonly IContentLibrary _contentLibrary;

        public float Volume
        {
            get => MediaPlayer.Volume;
            set => MediaPlayer.Volume = value;
        }

        public bool IsRepeating
        {
            get => MediaPlayer.IsRepeating;
            set => MediaPlayer.IsRepeating = value;
        }

        public string AssetPath
        {
            get => _assetPath;
            set
            {
                if (_assetPath == value)
                    return;

                _assetPath = value;
                AssetAsyncLoader.Asset = value != null ? _contentLibrary.GetAsset<Song>(value) : null;
            }
        }

        public SongPlayer(IContentLibrary contentLibrary)
        {
            _contentLibrary = contentLibrary;
        }

        public void LoadContent(IContentLibrary contentLibrary)
        {
            AssetAsyncLoader.LoadContent(CancellationToken.None);
        }

        public async Task LoadContentAsync(IContentLibrary contentLibrary)
        {
            await AssetAsyncLoader.LoadContentAsync(CancellationToken.None);
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (AssetAsyncLoader.UpdateContent(ref _song))
                MediaPlayer.Play(_song);
        }

        public void Resume()
        {
            MediaPlayer.Resume();
        }

        public void Pause()
        {
            MediaPlayer.Pause();
        }

        public override void Store()
        {
            AssetAsyncLoader.Store();
            base.Store();
        }

        public override void Restore()
        {
            base.Restore();
            AssetAsyncLoader.Restore();
        }

        public override void Dispose()
        {
            AssetAsyncLoader.Dispose();
            base.Dispose();
        }
    }
}