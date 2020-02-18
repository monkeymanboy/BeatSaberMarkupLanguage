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
                if (_notifyHost != null)
                {
                    _notifyHost.PropertyChanged -= NotifyHost_PropertyChanged;
                }
                _notifyHost = value;
                if (_notifyHost != null)
                {
                    _notifyHost.PropertyChanged -= NotifyHost_PropertyChanged;
                    _notifyHost.PropertyChanged += NotifyHost_PropertyChanged;
                }
            }
        }

        private void NotifyHost_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyInfo prop = sender.GetType().GetProperty(e.PropertyName);
            Action<object> action = null;
            if (ActionDict?.TryGetValue(e.PropertyName, out action) ?? false)
                action?.Invoke(prop.GetValue(sender));
        }

        private Dictionary<string, Action<object>> _actionDict;
        private Dictionary<string, Action<object>> ActionDict
        {
            get { return _actionDict; }
            set
            {
                if (_actionDict == value)
                    return;
                _actionDict = value;
            }
        }

        public bool AddAction(string propertyName, Action<object> action)
        {
            if (ActionDict == null)
                ActionDict = new Dictionary<string, Action<object>>();
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
            _actionDict?.Clear();
            NotifyHost = null;
        }
    }
}
