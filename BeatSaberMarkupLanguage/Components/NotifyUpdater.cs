using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    public class NotifyUpdater : MonoBehaviour
    {
        private readonly Dictionary<string, PropertyAction> actionDict = new();
        private INotifyPropertyChanged notifyHost;

        internal INotifyPropertyChanged NotifyHost
        {
            get => notifyHost;
            set
            {
                if (notifyHost != null)
                {
                    this.notifyHost.PropertyChanged -= NotifyHost_PropertyChanged;
                }

                notifyHost = value;

                if (notifyHost != null)
                {
                    this.notifyHost.PropertyChanged += NotifyHost_PropertyChanged;
                }
            }
        }

        internal bool AddAction(string propertyName, Action<object> action)
        {
            if (actionDict.TryGetValue(propertyName, out PropertyAction notify))
            {
                notify.AddAction(action);
            }
            else
            {
                PropertyInfo prop = notifyHost.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (prop == null)
                {
                    Logger.Log.Error($"No property '{propertyName}' on object of type '{notifyHost.GetType().FullName}'");
                    return false;
                }

                if (prop.GetMethod == null)
                {
                    Logger.Log.Error($"Property '{propertyName}' on object of type '{notifyHost.GetType().FullName}' does not have a getter");
                    return false;
                }

                actionDict.Add(propertyName, new PropertyAction(prop, action));
            }

            return true;
        }

        private void NotifyHost_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this == null)
            {
                this.notifyHost.PropertyChanged -= NotifyHost_PropertyChanged;
                return;
            }

            // https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.propertychangedeventargs.propertyname?view=netframework-4.7.2#remarks
            if (string.IsNullOrEmpty(e.PropertyName))
            {
                foreach (PropertyAction propertyAction in actionDict.Values)
                {
                    propertyAction.Invoke(sender);
                }
            }
            else if (actionDict.TryGetValue(e.PropertyName, out PropertyAction propertyAction))
            {
                propertyAction.Invoke(sender);
            }
        }

        private class PropertyAction
        {
            private readonly PropertyInfo property;

            private Action<object> action;

            internal PropertyAction(PropertyInfo property, Action<object> action)
            {
                this.property = property;
                this.action = action;
            }

            internal void AddAction(Action<object> newAction)
            {
                action += newAction;
            }

            internal void Invoke(object sender)
            {
                action.Invoke(property.GetValue(sender));
            }
        }
    }
}
