using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    public class NotifyUpdater : MonoBehaviour
    {
        private INotifyPropertyChanged notifyHost;

        public INotifyPropertyChanged NotifyHost
        {
            get => notifyHost;
            set
            {
                if (notifyHost != null)
                {
                    notifyHost.PropertyChanged -= NotifyHost_PropertyChanged;
                }

                notifyHost = value;

                if (notifyHost != null)
                {
                    notifyHost.PropertyChanged -= NotifyHost_PropertyChanged;
                    notifyHost.PropertyChanged += NotifyHost_PropertyChanged;
                }
            }
        }

        private void NotifyHost_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // OnDestroy is not called for disabled objects so this will make sure it is called if it gets called while destroyed
            if (this == null)
            {
                OnDestroy();
                return;
            }

            PropertyInfo prop = sender.GetType().GetProperty(e.PropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (ActionDict.TryGetValue(e.PropertyName, out Action<object> action))
            {
                action?.Invoke(prop.GetValue(sender));
            }
        }

        private Dictionary<string, Action<object>> ActionDict { get; set; } = new Dictionary<string, Action<object>>();

        public bool AddAction(string propertyName, Action<object> action)
        {
            ActionDict.Add(propertyName, action);
            return true;
        }

        public bool RemoveAction(string propertyName)
        {
            if (ActionDict != null)
            {
                return ActionDict.Remove(propertyName);
            }

            return false;
        }

        private void OnDestroy()
        {
            Logger.Log?.Debug($"NotifyUpdater destroyed.");
            ActionDict.Clear();
            NotifyHost = null;
        }
    }
}
