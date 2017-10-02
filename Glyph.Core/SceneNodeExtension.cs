namespace Glyph.Core
{
    static public class SceneNodeExtension
    {
        static public float DistanceTo(this ISceneNode a, ISceneNode b)
        {
            return (a.Position - b.Position).Length();
        }
    }
}