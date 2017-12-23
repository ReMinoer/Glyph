using Diese.Injection;

namespace Glyph.Injection
{
    public interface IInjectionClient
    {
        IDependencyInjector Injector { set; }
    }
}