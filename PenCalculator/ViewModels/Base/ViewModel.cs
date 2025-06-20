using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using System.Xaml;

namespace CV19Core.ViewModels.Base
{
    internal abstract class ViewModel : MarkupExtension, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private WeakReference _targetReference;
        private WeakReference _rootReference;

        public object TargetReference => _targetReference.Target;
        public object RootReference => _rootReference.Target;

        public void Dispose()
        {
            Dispose(true);
        }
        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed) return;
            _disposed = true;
            // Освобождение управляемых ресурсов
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Служит для обновления свойства для которого определено поле 
        /// в котором это свойство хранит свои данные
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">Ссылка на поле свойства</param>
        /// <param name="value">Новое значение для свойства</param>
        /// <param name="propertyName">Имя свойства для передачи в OnPropertyChanged</param>
        /// <returns></returns>
        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var valueTargetService = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            var rootObjectService = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;

            OnInitialized(valueTargetService?.TargetObject,
                          valueTargetService?.TargetProperty,
                          rootObjectService?.RootObject);

            return this;
        }

        protected virtual void OnInitialized(object target, object property, object root)
        {
            _targetReference = new WeakReference(target);
            _rootReference = new WeakReference(root);
        }
    }
}
