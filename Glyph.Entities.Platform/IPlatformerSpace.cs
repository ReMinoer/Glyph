namespace Glyph.Entities.Platform
{
    public interface IPlatformerSpace : ISpace
    {
        float Gravity { get; }

        void ManageCollision(PlatformCharacter perso);
    }
}