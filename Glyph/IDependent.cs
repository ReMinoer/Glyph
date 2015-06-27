namespace Glyph
{
    public interface IDependent
    {
        void BindDependencies(IDependencyProvider dependencyProvider);
    }
}