using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Glyph.PropertyChanged
{
    public class PropertyChangedObservable : IPropertyChangedObservable, IDisposable
    {
        private Subject<string> _changedPropertiesSubject;
        private IObservable<string> _changedProperties;
        protected Subject<string> ChangedPropertiesSubject => _changedPropertiesSubject ?? (_changedPropertiesSubject = SubjectPool.Instance.Get());
        
        [Browsable(false)]
        public IObservable<string> ChangedProperties => _changedProperties ?? (_changedProperties = ChangedPropertiesSubject.AsObservable());

        public virtual void Dispose()
        {
            if (_changedPropertiesSubject == null)
                return;

            ReconditionOrDispose();
        }

        protected void ReconditionOrDispose()
        {
            if (_changedProperties == null && !_changedPropertiesSubject.HasObservers)
                SubjectPool.Instance.Recondition(_changedPropertiesSubject);
            else
                _changedPropertiesSubject.Dispose();

            _changedPropertiesSubject = null;
        }

        protected bool Set<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (_changedPropertiesSubject == null)
            {
                backingField = value;
                return true;
            }
            
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return false;

            backingField = value;
            _changedPropertiesSubject.OnNext(propertyName);
            return true;
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            _changedPropertiesSubject?.OnNext(propertyName);
        }

        protected void NotifyPropertyChanged(params string[] propertyNames)
        {
            if (_changedPropertiesSubject == null)
                return;
            
            foreach (string propertyName in propertyNames)
                _changedPropertiesSubject.OnNext(propertyName);
        }
    }
}