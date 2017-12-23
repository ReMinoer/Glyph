using System;
using System.ComponentModel;

namespace Glyph.Observation.Properties
{
    public class ConfigurableNotifyPropertyChanged : PropertyChangedObservable, INotifyPropertyChanged
    {
        private readonly INotifyPropertyChangedConfiguration _configuration;
        private IDisposable _configurationSubscription;
        private IDisposable _eventSubscription;

        private event PropertyChangedEventHandler PrivatePropertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (PrivatePropertyChanged == null)
                    _configurationSubscription = _configuration.Operation.Subscribe(OnNextOperation, OnErrorOperation, OnCompletedOperation);
                
                PrivatePropertyChanged += value;
            }
            remove
            {
                PrivatePropertyChanged -= value;
                
                if (PrivatePropertyChanged == null)
                {
                    _configurationSubscription?.Dispose();
                    _configurationSubscription = null;

                    OnCompletedOperation();
                }
            }
        }

        public ConfigurableNotifyPropertyChanged()
        {
            _configuration = NotifyPropertyChangedConfiguration.Default;
        }

        public ConfigurableNotifyPropertyChanged(INotifyPropertyChangedConfiguration configuration)
        {
            _configuration = configuration;
        }

        private void OnNextOperation(NotifyPropertyChangedOperation operation)
        {
            _eventSubscription = (operation != null ? operation(ChangedPropertiesSubject) : ChangedPropertiesSubject).Subscribe(NotifyPrivatePropertyChanged);
        }

        private void OnCompletedOperation()
        {
            _eventSubscription.Dispose();
            _eventSubscription = null;
            
            ReconditionOrDispose();
        }

        private void OnErrorOperation(Exception _) => OnCompletedOperation();
        private void NotifyPrivatePropertyChanged(string propertyName) => PrivatePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public override void Dispose()
        {
            _configurationSubscription?.Dispose();
            _configurationSubscription = null;
            
            base.Dispose();
        }
    }
}