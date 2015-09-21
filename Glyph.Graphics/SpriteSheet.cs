using System.Collections.Generic;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class SpriteSheet : GlyphContainer, ISpriteSheet
    {
        private readonly SpriteLoader _spriteLoader;
        public ISpriteSheetCarver Carver { get; private set; }
        public List<Rectangle> Frames { get; private set; }

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

        public Rectangle this[int index]
        {
            get { return Frames[index]; }
        }

        public SpriteSheet()
            : base(1)
        {
            Components[0] = _spriteLoader = new SpriteLoader();
            Frames = new List<Rectangle>();
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            _spriteLoader.LoadContent(contentLibrary);

            if (Carver != null)
                ApplyCarver(Carver);
        }

        public Texture2D GetFrameTexture(int frameIndex)
        {
            return _spriteLoader.Texture;
        }

        public void ApplyCarver(ISpriteSheetCarver carver)
        {
            Carver = carver;

            Frames.Clear();
            carver.Process(this);
        }
    }
}