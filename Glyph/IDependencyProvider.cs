namespace Glyph
{
    public interface IDependencyProvider<in TAbstract>
    {
        T Resolve<T>() where T : class, TAbstract, new();
    }
}