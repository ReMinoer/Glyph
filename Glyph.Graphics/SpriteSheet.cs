using System;
using System.Collections.Generic;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class SpriteSheet : GlyphContainer, ISpriteSheet, ILoadContent
    {
        private readonly SpriteLoader _spriteLoader;
        private int _currentFrame;
        private bool _loadedContent;
        public int FramesCount => Frames.Count;
        public ISpriteSheetCarver Carver { get; private set; }
        public List<Rectangle> Frames { get; private set; }
        public Rectangle? Rectangle { get; private set; }

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

        public string Asset
        {
            get { return _spriteLoader.Asset; }
            set { _spriteLoader.Asset = value; }
        }

        public Texture2D Texture
        {
            get { return _spriteLoader.Texture; }
        }

        public event Action<ISpriteSource> Loaded;

        public SpriteSheet()
        {
            _spriteLoader = new SpriteLoader();
            Components.Add(_spriteLoader);

            Frames = new List<Rectangle>();
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            _spriteLoader.LoadContent(contentLibrary);

            if (Carver != null)
                ApplyCarver(Carver);

            Refresh();
            _loadedContent = true;

            if (Loaded != null)
                Loaded.Invoke(this);
        }

        public void ApplyCarver(ISpriteSheetCarver carver)
        {
            Carver = carver;
            Frames.Clear();

            if (Texture != null)
                carver.Process(this);
        }

        public Rectangle GetFrameRectangle(int frameIndex)
        {
            return Frames[frameIndex];
        }

        public Rectangle GetDrawnRectagle()
        {
            return Rectangle ?? _spriteLoader.GetDrawnRectagle();
        }

        Vector2 ISpriteSource.GetDefaultOrigin()
        {
            return (Rectangle?.Size.ToVector2() ?? _spriteLoader.Texture.Size()) / 2;
        }

        Texture2D ISpriteSheet.GetFrameTexture(int frameIndex)
        {
            return Texture;
        }

        private void Refresh()
        {
            Rectangle = GetFrameRectangle(CurrentFrame);
        }
    }
}