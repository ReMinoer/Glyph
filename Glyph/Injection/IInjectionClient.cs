using Niddle;

namespace Glyph.Injection
{
    public interface IInjectionClient
    {
        IDependencyInjector Injector { set; }
    }
}