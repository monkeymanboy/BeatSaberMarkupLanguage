using BeatSaberMarkupLanguage.Notify;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    public class NotifyUpdater : MonoBehaviour
    {
        private INotifiableHost _notifyHost;
        public INotifiableHost NotifyHost
        {
            get { return _notifyHost; }
            set
            {
                if ((object)_notifyHost != null)
                {
                    _notifyHost.PropertyChanged -= NotifyHost_PropertyChanged;
                }
                _notifyHost = value;
                if ((object)_notifyHost != null)
                {
                    _notifyHost.PropertyChanged -= NotifyHost_PropertyChanged;
                    _notifyHost.PropertyChanged += NotifyHost_PropertyChanged;
                }
            }
        }

        private void NotifyHost_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this == null) //OnDestroy is not called for disabled objects so this will make sure it is called if it gets called while destroyed
            {
                OnDestroy();
                return;
            }
            PropertyInfo prop = sender.GetType().GetProperty(e.PropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Action<object> action;
            if (ActionDict.TryGetValue(e.PropertyName, out action))
            {
                if (action != null)
                {
                    action.Invoke(prop.GetValue(sender));
                    ComponentChanged?.Invoke(this, null);
                }
            }
        }

        public event EventHandler ComponentChanged;

        private Dictionary<string, Action<object>> ActionDict{ get; set; } = new Dictionary<string, Action<object>>();

        public bool AddAction(string propertyName, Action<object> action)
        {
            ActionDict.Add(propertyName, action);
            return true;
        }

        public bool RemoveAction(string propertyName)
        {
            if (ActionDict != null)
                return ActionDict.Remove(propertyName);
            return false;
        }

        void OnDestroy()
        {
            Logger.log?.Debug($"NotifyUpdater destroyed.");
            ActionDict.Clear();
            NotifyHost = null;
        }
    }
}
