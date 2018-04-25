using System.Collections.Generic;
using Simulacra;

namespace Glyph.Composition.Modelization
{
    public class CompositeData<T, TData> : List<TData>, IConfigurator<IGlyphComposite<T>>, IDataModel<IGlyphComposite<T>>
        where T : class, IGlyphComponent
        where TData : GlyphData<T>, new()
    {
        public void From(IGlyphComposite<T> obj)
        {
            Clear();

            foreach (T item in obj.Components)
            {
                var data = new TData();
                data.From(item);
                Add(data);
            }
        }

        public void Configure(IGlyphComposite<T> obj)
        {
            obj.Clear();

            foreach (TData data in this)
                obj.Add(data.Create());
        }
    }
}