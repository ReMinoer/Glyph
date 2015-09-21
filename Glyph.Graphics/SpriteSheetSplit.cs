using System.Collections.Generic;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public sealed class SpriteSheetSplit : GlyphComposite<SpriteSheet>, ISpriteSheet
    {
        public ISpriteSheetCarver Carver { get; private set; }
        public List<FrameData> Frames { get; private set; }

        public Rectangle this[int index]
        {
            get { return Frames[index].Bounds; }
        }

        public SpriteSheetSplit(params string[] assets)
        {
            Frames = new List<FrameData>();
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