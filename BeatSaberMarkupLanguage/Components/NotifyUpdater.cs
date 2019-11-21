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

                Logger.log?.Warn($"PropertyChanged: {e.PropertyName}, but not active. ({gameObject.GetInstanceID()}.{GetInstanceID()})");
                return;
            }
            Logger.log?.Warn($"PropertyChanged: {e.PropertyName} ({gameObject.name} - {gameObject.GetInstanceID()}.{GetInstanceID()})");
            var prop = sender.GetType().GetProperty(e.PropertyName);
            string val = string.Empty;
            if (ActionDict.TryGetValue(e.PropertyName, out var action))
            {
                val = prop.GetValue(sender).ToString();
                action?.Invoke(prop.GetValue(sender));
            }
            Logger.log?.Warn($"     New Value: {val}");
        }

        private Dictionary<string, Action<object>> _actionDict;
        public Dictionary<string, Action<object>> ActionDict
        {
            get { return _actionDict; }
            set
            {
                if (_actionDict == value)
                    return;
                _actionDict = value;
            }
        }

        void OnEnable()
        {
            Logger.log?.Warn($"NotifyUpdater enabled. {isActiveAndEnabled}");
        }

        void OnDisable()
        {
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
