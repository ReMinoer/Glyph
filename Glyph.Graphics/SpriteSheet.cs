using System.Collections.Generic;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class SpriteSheet : GlyphContainer, ISpriteSheet, ILoadContent
    {
        private readonly SpriteTransformer _spriteTransformer;
        private readonly SpriteLoader _spriteLoader;
        private int _currentFrame;
        public ISpriteSheetCarver Carver { get; private set; }
        public List<Rectangle> Frames { get; private set; }

        public int CurrentFrame
        {
            get { return _currentFrame; }
            set
            {
                _currentFrame = value;
                _spriteTransformer.SourceRectangle = CurrentRectangle;
            }
        }

        public int Count
        {
            get { return Frames.Count; }
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

        public Rectangle CurrentRectangle
        {
            get { return GetFrameRectangle(CurrentFrame); }
        }

        public SpriteSheet(SpriteTransformer spriteTransformer)
            : base(1)
        {
            _spriteTransformer = spriteTransformer;
            Components[0] = _spriteLoader = new SpriteLoader();
            Frames = new List<Rectangle>();
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            _spriteLoader.LoadContent(contentLibrary);

            if (Carver != null)
                ApplyCarver(Carver);
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

        Texture2D ISpriteSheet.GetFrameTexture(int frameIndex)
        {
            return Texture;
        }
    }
}