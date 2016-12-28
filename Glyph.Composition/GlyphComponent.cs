using System.ComponentModel;
using System.Runtime.CompilerServices;
using Diese;
using Glyph.Composition.Annotations;
using Stave;

namespace Glyph.Composition
{
    public class GlyphComponent : Component<IGlyphComponent, IGlyphParent>, IGlyphComponent
    {
        public string Name { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public GlyphComponent()
        {
            Name = GetType().GetDisplayName();
            InstanceManager.ConstructorProcess(this);
        }

        public virtual void Initialize()
        {
        }

        public virtual void Dispose()
        {
            InstanceManager.DisposeProcess(this);
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
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