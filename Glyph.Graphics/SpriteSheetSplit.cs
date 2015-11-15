using System;
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

        public void Add(string asset)
        {
            Add(new SpriteSheet { Asset = asset });
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            foreach (SpriteSheet spriteSheet in this)
            {
                spriteSheet.LoadContent(contentLibrary);

                if (Carver != null)
                    spriteSheet.ApplyCarver(Carver);
            }

            Refresh();
            _loadedContent = true;
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

            foreach (SpriteSheet spriteSheet in this)
                spriteSheet.ApplyCarver(carver);
        }

        Rectangle ISpriteSource.GetDrawnRectangle()
        {
            return Rectangle.GetValueOrDefault();
        }

        private FrameData GetFrameData(int frameIndex)
        {
            int sum = 0;
            foreach (SpriteSheet spriteSheet in this)
            {
                sum += spriteSheet.Count;

                if (frameIndex >= sum)
                    continue;

                int localFrame = frameIndex - (sum - spriteSheet.Count);

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