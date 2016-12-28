using System.ComponentModel;
using System.Runtime.CompilerServices;
using Diese;
using Glyph.Composition.Annotations;
using Stave;

namespace Glyph.Composition
{
    public abstract class GlyphContainer : GlyphContainer<IGlyphComponent>
    {
    }

    public abstract class GlyphContainer<TComponent> : Container<IGlyphComponent, IGlyphParent, TComponent>, IGlyphContainer<TComponent>
        where TComponent : class, IGlyphComponent
    {
        public string Name { get; set; }
        public bool IsFreeze { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected GlyphContainer()
        {
            Name = GetType().GetDisplayName();
            InstanceManager.ConstructorProcess(this);
        }

        public virtual void Initialize()
        {
        }

        public virtual void Dispose()
        {
            foreach (TComponent component in Components)
                component.Dispose();

            InstanceManager.DisposeProcess(this);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged(params string[] propertyNames)
        {
            for (int i = 0; i < propertyNames.Length; i++)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyNames[i]));
        }
    }
}