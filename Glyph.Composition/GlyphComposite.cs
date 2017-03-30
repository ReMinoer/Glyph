using System.ComponentModel;
using System.Runtime.CompilerServices;
using Diese;
using Glyph.Composition.Annotations;
using Stave;

namespace Glyph.Composition
{
    public abstract class GlyphComposite : GlyphComposite<IGlyphComponent>
    {
    }

    public abstract class GlyphComposite<TComponent> : Composite<IGlyphComponent, IGlyphParent, TComponent>, IGlyphComposite<TComponent>
        where TComponent : class, IGlyphComponent
    {
        public string Name { get; set; }
        public bool IsFreeze { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected GlyphComposite()
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

            Clear();

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
            foreach (string name in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}