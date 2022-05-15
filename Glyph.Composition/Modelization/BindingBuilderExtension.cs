using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml;
using Glyph.IO;
using Simulacra.Binding;

namespace Glyph.Composition.Modelization
{
    static public class BindingBuilderExtension
    {
        static public IBindingBuilder<TModel, TView, TLoadedValue, TBindingCollection> Load<TModel, TView, TLoadedValue, TBindingCollection>(
            this IBindingBuilder<TModel, TView, string, TBindingCollection> builder,
            Func<ILoadFormat<TLoadedValue>> loadFormatProvider)
        {
            return builder.Select((mv, m, v) => SafeLoad(mv, stream =>
            {
                ILoadFormat<TLoadedValue> serializationFormat = loadFormatProvider();
                return serializationFormat.Load(stream);
            }));
        }

        static public IBindingBuilder<TModel, TView, TLoadedValue, TBindingCollection> Load<TModel, TView, TLoadedValue, TBindingCollection>(
            this IBindingBuilder<TModel, TView, string, TBindingCollection> builder,
            Func<ISerializationFormat<TLoadedValue>> serializationFormatProvider)
            where TModel : IGlyphData
        {
            return builder.Select((mv, m, v) => SafeLoad(mv, stream =>
            {
                ISerializationFormat<TLoadedValue> serializationFormat = serializationFormatProvider();
                serializationFormat.KnownTypes = m.SerializationKnownTypes;
                return serializationFormat.Load(stream);
            }));
        }

        static public IBindingBuilder<TModel, TView, TLoadedValue, TBindingCollection> Load<TModel, TView, TLoadedValue, TBindingCollection>(
            this IBindingBuilder<TModel, TView, FilePath, TBindingCollection> builder,
            Func<ILoadFormat<TLoadedValue>> loadFormatProvider)
        {
            return builder.Select((mv, m, v) => SafeLoad(mv, stream =>
            {
                ILoadFormat<TLoadedValue> serializationFormat = loadFormatProvider();
                return serializationFormat.Load(stream);
            }));
        }

        static public IBindingBuilder<TModel, TView, TLoadedValue, TBindingCollection> Load<TModel, TView, TLoadedValue, TBindingCollection>(
            this IBindingBuilder<TModel, TView, FilePath, TBindingCollection> builder,
            Func<ISerializationFormat<TLoadedValue>> serializationFormatProvider)
            where TModel : IGlyphData
        {
            return builder.Select((mv, m, v) => SafeLoad(mv, stream =>
            {
                ISerializationFormat<TLoadedValue> serializationFormat = serializationFormatProvider();
                serializationFormat.KnownTypes = m.SerializationKnownTypes;
                return serializationFormat.Load(stream);
            }));
        }

        static private TLoadedValue SafeLoad<TLoadedValue>(string path, Func<Stream, TLoadedValue> loader)
        {
            if (!File.Exists(path))
                return default;

            Stream stream;
            const int maxAccessTry = 100;
            for (int i = 0;; i++)
            {
                try
                {
                    stream = File.OpenRead(path);
                    break;
                }
                catch (IOException)
                {
                    Thread.Sleep(10);
                    if (i == maxAccessTry)
                        throw;
                }
            }

            using (stream)
            {
                try
                {
                    return loader(stream);
                }
                catch (XmlException)
                {
                    return default;
                }
                catch (SerializationException)
                {
                    return default;
                }
                finally
                {
                    stream.Close();
                }
            }
        }

        static public IBindingBuilder<TModel, TView, IGlyphComponent, TBindingCollection> CreateComponent<TModel, TView, TCreator, TBindingCollection>(
            this IBindingBuilder<TModel, TView, TCreator, TBindingCollection> builder)
            where TModel : IGlyphData, IHierarchicalData
            where TCreator : IGlyphCreator<IGlyphComponent>
        {
            return builder.CreateComponent<TModel, TView, TCreator, IGlyphComponent, TBindingCollection>();
        }

        static public IBindingBuilder<TModel, TView, TCreated, TBindingCollection> CreateComponent<TModel, TView, TCreator, TCreated, TBindingCollection>(
            this IBindingBuilder<TModel, TView, TCreator, TBindingCollection> builder)
            where TModel : IGlyphData
            where TCreator : IGlyphCreator<TCreated>
            where TCreated : IGlyphComponent
        {
            return builder.Select((mv, m, v) =>
            {
                if (mv == null)
                    return default;

                mv.SerializationKnownTypes = m.SerializationKnownTypes;
                mv.DependencyResolver = m.DependencyResolver;

                if (m.NotBinding)
                    return mv.Create();

                mv.Instantiate();
                return mv.BindedObject;
            });
        }

        static public void ToBindedComposite<TModel, TView, TModelItem>(this ICollectionBindingBuilder<TModel, TView, TModelItem, TModelItem> builder)
            where TModel : IGlyphData, IHierarchicalData
            where TView : IGlyphComposite<IGlyphComponent>
            where TModelItem : class, IGlyphCreator<IGlyphComponent>
        {
            builder.ToComposite(v => v);
        }

        static public void ToComposite<TModel, TView, TModelItem>(
            this ICollectionBindingBuilder<TModel, TView, TModelItem, TModelItem> builder,
            Func<TView, IGlyphComposite<IGlyphComponent>> compositeGetter)
            where TModel : IGlyphData, IHierarchicalData
            where TModelItem : class, IGlyphCreator<IGlyphComponent>
        {
            builder.BindingCollection.Add(builder.ReferencePropertyName, new OneWayFactoryBinding<TModel, TView, TModelItem, IGlyphComponent>(
                builder.ReferenceGetter,
                (m, mi, v) => mi,
                compositeGetter,
                builder.ReferencePropertyName)
                .AsSubscriptionBinding(builder.SubscriptionGetter));
        }
    }
}