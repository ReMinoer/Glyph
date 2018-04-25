using System;
using System.Collections.Generic;
using Niddle;
using Glyph.Composition;
using Simulacra;

namespace Glyph.Core.Tracking
{
    public class FactoryData<T> : List<T>, IDataModel<Factory<T>>, IConfigurator<Factory<T>>
        where T : class, IGlyphComponent, new()
    {
        public void From(Factory<T> obj)
        {
            Clear();
            foreach (T element in obj)
                Add(element);
        }

        public void Configure(Factory<T> obj)
        {
            obj.Clear();

            foreach (T item in this)
                obj.Register(item);
        }
    }

    public class FactoryData<T, TData> : List<TData>, IDataModel<Factory<T>>, IConfigurator<Factory<T>>
        where TData : GlyphData<T>, new()
        where T : class, IGlyphComponent
    {
        public Action<TData> DataConfiguration { get; private set; }

        public FactoryData(IDependencyInjector injector)
        {
            DataConfiguration = x => x.Injector = injector;
        }

        public void From(Factory<T> obj)
        {
            Clear();
            foreach (T element in obj)
            {
                var dataModel = new TData();
                dataModel.From(element);
                Add(dataModel);
            }
        }

        public void Configure(Factory<T> obj)
        {
            obj.Clear();

            foreach (TData data in this)
                obj.Register(CreateItem(data));
        }

        private T CreateItem(TData data)
        {
            DataConfiguration?.Invoke(data);

            T item = data.Create();
            return item;
        }
    }
}