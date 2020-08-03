using System;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Simulacra.Binding;

namespace Glyph.Core.Modelization
{
    static public class BindingBuilderExtension
    {
        static public void SetToBindedObject<TModel, TView, TComponent, TBindingCollection>(this IBindingBuilder<TModel, TView, TComponent, TBindingCollection> builder)
            where TView : GlyphObject
            where TComponent : IGlyphComponent
        {
            builder.SetToObject(v => v);
        }

        static public void SetToObject<TModel, TView, TComponent, TBindingCollection>(
            this IBindingBuilder<TModel, TView, TComponent, TBindingCollection> builder,
            Func<TView, GlyphObject> objectGetter)
            where TView : class, IGlyphComponent
            where TComponent : IGlyphComponent
        {
            builder.Do((m, v) => objectGetter(v).SetComponent('%' + builder.Name, builder.Getter(m, v)));
        }
    }
}