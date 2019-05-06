using Niddle;

namespace Glyph.Resolver
{
    public interface IResolveClient
    {
        IDependencyResolver Resolver { set; }
    }
}