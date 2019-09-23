using System;
using System.Linq;
using System.Threading.Tasks;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public sealed class SpriteSheetSplit : GlyphComposite<SpriteSheet>, ISpriteSheet, ILoadContent
    {
        private bool _loadedContent;
        private int _currentFrame;
        private FrameData _frameData;
        public int FramesCount => Components.Sum(x => x.FramesCount);
        public ISpriteSheetCarver Carver { get; private set; }

        public int CurrentFrame
        {
            get { return _currentFrame; }
            set
            {
                _currentFrame = value;
                if (_loadedContent)
                    Refresh();
            }
        }

        public Rectangle? Rectangle
        {
            get { return _frameData.Rectangle; }
        }

        public Texture2D CurrentTexture
        {
            get { return _frameData.Texture; }
        }

        Texture2D ISpriteSource.Texture
        {
            get { return CurrentTexture; }
        }

        public event Action<ISpriteSource> Loaded;

        public void Add(string asset)
        {
            Add(new SpriteSheet { Asset = asset });
        }

        public async Task LoadContent(IContentLibrary contentLibrary)
        {
            await Task.WhenAll(Components.Select(async x => await x.LoadContent(contentLibrary)));
            
            if (Carver != null)
                foreach (SpriteSheet spriteSheet in Components)
                    spriteSheet.ApplyCarver(Carver);

            Refresh();
            _loadedContent = true;

            Loaded?.Invoke(this);
        }

        public Rectangle GetFrameRectangle(int frameIndex)
        {
            return GetFrameData(frameIndex).Rectangle;
        }

        public Texture2D GetFrameTexture(int frameIndex)
        {
            return GetFrameData(frameIndex).Texture;
        }

        public void ApplyCarver(ISpriteSheetCarver carver)
        {
            Carver = carver;

            foreach (SpriteSheet spriteSheet in Components)
                spriteSheet.ApplyCarver(carver);
        }

        public Rectangle GetDrawnRectangle()
        {
            return Rectangle ?? CurrentTexture.Bounds;
        }

        Vector2 ISpriteSource.GetDefaultOrigin()
        {
            return (Rectangle?.Size.ToVector2() ?? CurrentTexture.Size()) / 2;
        }

        private FrameData GetFrameData(int frameIndex)
        {
            int sum = 0;
            foreach (SpriteSheet spriteSheet in Components)
            {
                sum += spriteSheet.FramesCount;

                if (frameIndex >= sum)
                    continue;

                int localFrame = frameIndex - (sum - spriteSheet.FramesCount);

                return new FrameData
                {
                    Texture = spriteSheet.Texture,
                    Rectangle = spriteSheet.GetFrameRectangle(localFrame)
                };
            }

            throw new ArgumentOutOfRangeException();
        }

        private void Refresh()
        {
            _frameData = GetFrameData(_currentFrame);
        }

        private struct FrameData
        {
            public Texture2D Texture { get; set; }
            public Rectangle Rectangle { get; set; }
        }
    }
}