using Caliburn.Micro;
using global::ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Caliburn.Micro.ReactiveUI
{
    public class ReactiveObjectEx : ReactiveObject, INotifyPropertyChangedEx
    {
        /// <summary>
        /// Enables/Disables property change notification.
        /// Virtualized in order to help with document oriented view models.
        /// </summary>
        [Obsolete("Use ReactiveUI's SuppressNotifications")]
        public virtual bool IsNotifying
        {
            get => this.AreChangeNotificationsEnabled();
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// Raises a change notification indicating that all bindings should be refreshed.
        /// </summary>
        public virtual void Refresh()
        {
            this.RaisePropertyChanged(string.Empty);
        }

        /// <summary>
        /// Notifies subscribers of the property change.
        /// </summary>
        /// <param name = "propertyName">Name of the property.</param>
        public virtual void NotifyOfPropertyChange([CallerMemberName] string propertyName = null)
        {
            this.RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Notifies subscribers of the property change.
        /// </summary>
        /// <typeparam name = "TProperty">The type of the property.</typeparam>
        /// <param name = "property">The property expression.</param>
        public void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property)
        {
            this.RaisePropertyChanged(property.GetMemberInfo().Name);
        }

        public bool Set<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            var changed = EqualityComparer<T>.Default.Equals(oldValue, newValue);

            this.RaiseAndSetIfChanged(ref oldValue, newValue, propertyName);

            return changed;
        }

        public bool SetAndNotify<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            var changed = EqualityComparer<T>.Default.Equals(oldValue, newValue);

            this.RaiseAndSetIfChanged(ref oldValue, newValue, propertyName);

            return changed;
        }
    }
}
