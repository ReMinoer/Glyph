using Microsoft.Xna.Framework;

namespace Glyph.Effects.Masked
{
    public struct MaskPatch
    {
        public int PatchIndex;
        public Vector2 Center;
        public float Size;

        public MaskPatch(Vector2 center, float size, int patchIndex = 0)
        {
            PatchIndex = patchIndex;
            Center = center;
            Size = size;
        }
    }
}