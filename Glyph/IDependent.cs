namespace Glyph
{
    public interface IDependent<out TAbstract>
    {
        void BindDependencies(IDependencyProvider<TAbstract> dependencyProvider);
    }
}