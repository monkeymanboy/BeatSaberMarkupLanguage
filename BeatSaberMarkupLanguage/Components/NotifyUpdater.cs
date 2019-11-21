using BeatSaberMarkupLanguage.Notify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                if (_notifyHost == value)
                    return;
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
            if (!isActiveAndEnabled) // TODO: Better to subscribe/unsubscribe OnEnable/OnDisable?
            {
                Logger.log.Error($"Shouldn't ever see this, event was unsubscribed in OnDisable()");
                return;
            }
            var prop = sender.GetType().GetProperty(e.PropertyName);
            string val = string.Empty;
            Action<object> action = null;
            if (ActionDict?.TryGetValue(e.PropertyName, out action) ?? false)
            {

                Logger.log?.Warn($"PropertyChanged: {e.PropertyName} ({gameObject.name} - {gameObject.GetInstanceID()}.{GetInstanceID()})");
                val = prop.GetValue(sender).ToString();
                action?.Invoke(prop.GetValue(sender));
                Logger.log?.Warn($"     New Value: {val}");
            }
            else
                Logger.log?.Warn($"{gameObject.name}: No Action defined for {e.PropertyName}");
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

        void OnEnable()
        {
            Logger.log?.Warn($"NotifyUpdater enabled. {isActiveAndEnabled}");
            if (NotifyHost != null)
            {
                NotifyHost.PropertyChanged -= NotifyHost_PropertyChanged;
                NotifyHost.PropertyChanged += NotifyHost_PropertyChanged;
            }
        }

        void OnDisable()
        {
            if (NotifyHost != null)
            {
                NotifyHost.PropertyChanged -= NotifyHost_PropertyChanged;
            }
            Logger.log?.Warn($"NotifyUpdater disabled.");
        }

        void OnDestroy()
        {
            Logger.log?.Warn($"NotifyUpdater destroyed.");
            _actionDict?.Clear();
            NotifyHost = null;
        }
    }
}
