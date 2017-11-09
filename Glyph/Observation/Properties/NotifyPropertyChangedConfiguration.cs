using System.Reactive.Subjects;

namespace Glyph.Observation.Properties
{
    public class NotifyPropertyChangedConfiguration : INotifyPropertyChangedConfiguration
    {
        static private NotifyPropertyChangedConfiguration _default;
        static public NotifyPropertyChangedConfiguration Default => _default ?? (_default = new NotifyPropertyChangedConfiguration());
        
        public BehaviorSubject<NotifyPropertyChangedOperation> Operation { get; } = new BehaviorSubject<NotifyPropertyChangedOperation>(null);
    }
    
}