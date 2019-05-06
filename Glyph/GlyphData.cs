using Niddle;
using Simulacra;

namespace Glyph
{
    public abstract class GlyphData<T> : IDataModel<T>, ICreator<T>, IConfigurator<T>
    {
        public IDependencyResolver Resolver { protected get; set; }

        public abstract void From(T obj);
        public abstract void Configure(T obj);

        public T Create()
        {
            var obj = Resolver.Resolve<T>();
            Configure(obj);
            return obj;
        }
    }
}