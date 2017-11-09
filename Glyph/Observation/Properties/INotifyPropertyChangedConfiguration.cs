using System;
using System.Reactive.Subjects;

namespace Glyph.Observation.Properties
{
    public delegate IObservable<string> NotifyPropertyChangedOperation(IObservable<string> observable);
    
    public interface INotifyPropertyChangedConfiguration
    {
        BehaviorSubject<NotifyPropertyChangedOperation> Operation { get; }
    }
}