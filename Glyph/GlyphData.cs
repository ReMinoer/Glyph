using Diese.Injection;
using Diese.Modelization;

namespace Glyph
{
    public abstract class GlyphData<T> : ICreationData<T>, IConfigurationData<T>
    {
        public IDependencyInjector Injector { protected get; set; }

        public abstract void From(T obj);
        public abstract void Configure(T obj);

        public T Create()
        {
            var obj = Injector.Resolve<T>();
            Configure(obj);
            return obj;
        }
    }
}