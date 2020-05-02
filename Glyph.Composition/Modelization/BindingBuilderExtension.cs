using System;
using System.IO;
using System.Runtime.Serialization;
using Glyph.IO;
using Simulacra.Binding;

namespace Glyph.Composition.Modelization
{
    static public class BindingBuilderExtension
    {
        static public PropertyBindingBuilder<TModel, TView, TLoadedValue> Load<TModel, TView, TLoadedValue>(
            this IPropertyBindingBuilder<TModel, TView, AssetPath> builder,
            Func<ILoadFormat<TLoadedValue>> loadFormatProvider)
        {
            return builder.Select((mv, m, v) =>
            {
                if (!File.Exists(mv.Path))
                    return default;

                using (Stream stream = File.OpenRead(mv.Path))
                {
                    try
                    {
                        return loadFormatProvider().Load(stream);
                    }
                    catch (SerializationException)
                    {
                        return default;
                    }
                }
            });
        }

        static public PropertyBindingBuilder<TModel, TView, TLoadedValue> Load<TModel, TView, TLoadedValue>(
            this IPropertyBindingBuilder<TModel, TView, AssetPath> builder,
            Func<ISerializationFormat<TLoadedValue>> serializationFormatProvider)
            where TModel : BindedData<TModel, TView>
            where TView : class, IGlyphComponent
        {
            return builder.Select((mv, m, v) =>
            {
                if (!File.Exists(mv.Path))
                    return default;

                ISerializationFormat<TLoadedValue> serializationFormat = serializationFormatProvider();
                serializationFormat.KnownTypes = m.SerializationKnownTypes;

                using (Stream stream = File.OpenRead(mv.Path))
                {
                    try
                    {
                        return serializationFormat.Load(stream);
                    }
                    catch (SerializationException)
                    {
                        return default;
                    }
                }
            });
        }

        static public PropertyBindingBuilder<TModel, TView, IGlyphComponent> CreateComponent<TModel, TView, TCreator>(
            this IPropertyBindingBuilder<TModel, TView, TCreator> builder)
            where TModel : BindedData<TModel, TView>
            where TView : class, IGlyphComponent
            where TCreator : IGlyphCreator<IGlyphComponent>
        {
            return builder.Create<TModel, TView, TCreator, IGlyphComponent>();
        }

        static public PropertyBindingBuilder<TModel, TView, TCreated> CreateComponent<TModel, TView, TCreator, TCreated>(
            this IPropertyBindingBuilder<TModel, TView, TCreator> builder)
            where TModel : BindedData<TModel, TView>
            where TView : class, IGlyphComponent
            where TCreator : IGlyphCreator<TCreated>
            where TCreated : IGlyphComponent
        {
            return builder.Select((mv, m, v) =>
            {
                if (mv == null)
                    return default;

                mv.SerializationKnownTypes = m.SerializationKnownTypes;
                mv.DependencyResolver = m.Resolver;
                return mv.Create();
            });
        }

        static public void ToBindedComposite<TModel, TView, TModelItem>(this ICollectionBindingBuilder<TModel, TView, TModelItem, TModelItem> builder)
            where TModel : BindedData<TModel, TView>
            where TView : class, IGlyphComposite<IGlyphComponent>
            where TModelItem : IGlyphCreator<IGlyphComponent>
        {
            builder.ToComposite(v => v);
        }

        static public void ToComposite<TModel, TView, TModelItem>(
            this ICollectionBindingBuilder<TModel, TView, TModelItem, TModelItem> builder,
            Func<TView, IGlyphComposite<IGlyphComponent>> compositeGetter)
            where TModel : BindedData<TModel, TView>
            where TView : class, IGlyphComponent
            where TModelItem : IGlyphCreator<IGlyphComponent>
        {
            builder.BindingCollection.Add(builder.ReferencePropertyName, new OneWayFactoryBinding<TModel, TView, TModelItem, IGlyphComponent>(
                builder.ReferenceGetter,
                (m, mi, v) => mi,
                compositeGetter)
                .AsEventBinding(builder.EventSourceGetter));
        }
    }
}