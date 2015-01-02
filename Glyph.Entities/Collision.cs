namespace Glyph.Entities
{
    public struct Collision
    {
        public bool IsWallLeft { get; set; }
        public bool IsWallRight { get; set; }
        public bool IsFloor { get; set; }
        public bool IsCeiling { get; set; }
        public float NewPosX { get; set; }
        public float NewPosY { get; set; }
        public float NewDynamiqueX { get; set; }
        public float NewDynamiqueY { get; set; }
    }

    public enum ObstructionType
    {
        None,
        Floor,
        Ceiling,
        WallLeft,
        WallRight
    }
}