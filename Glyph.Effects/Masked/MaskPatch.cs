using Microsoft.Xna.Framework;

namespace Glyph.Effects.Masked
{
    public struct MaskPatch
    {
        public int PatchIndex { get; set; }
        public Vector2 Center { get; set; }
        public float Size { get; set; }

        public MaskPatch(Vector2 center, float size, int patchIndex = 0)
        {
            PatchIndex = patchIndex;
            Center = center;
            Size = size;
        }
    }
}