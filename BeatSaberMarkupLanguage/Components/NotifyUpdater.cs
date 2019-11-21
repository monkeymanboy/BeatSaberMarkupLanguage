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
                return;
            var prop = sender.GetType().GetProperty(e.PropertyName);
            if (ActionDict.TryGetValue(e.PropertyName, out var action))
                action?.Invoke(prop.GetValue(sender));

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

        void OnDestroy()
        {
            _actionDict?.Clear();
            NotifyHost = null;
        }
    }
}
