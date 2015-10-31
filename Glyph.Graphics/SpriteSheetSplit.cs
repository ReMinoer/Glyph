using System.Collections.Generic;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public sealed class SpriteSheetSplit : GlyphComposite<SpriteSheet>, ISpriteSheet, ILoadContent
    {
        private int _currentFrame;
        private readonly SpriteTransformer _spriteTransformer;
        public ISpriteSheetCarver Carver { get; private set; }
        public List<FrameData> Frames { get; private set; }

        public int CurrentFrame
        {
            get { return _currentFrame; }
            set
            {
                _currentFrame = value;
                _spriteTransformer.SourceRectangle = CurrentRectangle;
            }
        }

        public Rectangle CurrentRectangle
        {
            get { return GetFrameRectangle(CurrentFrame); }
        }

        public Texture2D CurrentTexture
        {
            get { return Frames[CurrentFrame].Texture; }
        }

        Texture2D ISpriteSource.Texture
        {
            get { return CurrentTexture; }
        }

        public SpriteSheetSplit(SpriteTransformer spriteTransformer)
        {
            _spriteTransformer = spriteTransformer;
            Frames = new List<FrameData>();
        }

        public void Add(string asset)
        {
            Add(new SpriteSheet(_spriteTransformer) { Asset = asset });
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            foreach (SpriteSheet spriteSheet in this)
            {
                spriteSheet.LoadContent(contentLibrary);

                if (Carver != null)
                    spriteSheet.ApplyCarver(Carver);
            }
        }

        public Rectangle GetFrameRectangle(int frameIndex)
        {
            return Frames[frameIndex].Bounds;
        }

        public Texture2D GetFrameTexture(int frameIndex)
        {
            return Frames[frameIndex].Texture;
        }

        public void ApplyCarver(ISpriteSheetCarver carver)
        {
            Carver = carver;

            Frames.Clear();
            foreach (SpriteSheet spriteSheet in this)
                spriteSheet.ApplyCarver(carver);
        }

        public struct FrameData
        {
            public Texture2D Texture { get; set; }
            public Rectangle Bounds { get; set; }
        }
    }
}