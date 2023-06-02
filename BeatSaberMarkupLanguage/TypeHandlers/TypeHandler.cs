using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using UnityEngine;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    public abstract class TypeHandler
    {
        private Dictionary<string, string[]> cachedProps;

        public Dictionary<string, string[]> CachedProps => cachedProps ??= Props;

        public abstract Dictionary<string, string[]> Props { get; }

        public abstract void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams);

        public virtual void HandleTypeAfterChildren(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
        }

        public virtual void HandleTypeAfterParse(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
        }
    }

    public abstract class TypeHandler<T> : TypeHandler
        where T : Component
    {
        private Dictionary<string, Action<T, string>> cachedSetters;

        public Dictionary<string, Action<T, string>> CachedSetters => cachedSetters ??= Setters;

        public abstract Dictionary<string, Action<T, string>> Setters { get; }

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            if (componentType.component is T obj)
            {
                NotifyUpdater updater = null;
                foreach (KeyValuePair<string, string> pair in componentType.data)
                {
                    if (CachedSetters.TryGetValue(pair.Key, out Action<T, string> action))
                    {
                        action.Invoke(obj, pair.Value);
                        if (componentType.valueMap.TryGetValue(pair.Key, out BSMLValue value))
                        {
                            updater = BindValue(componentType, parserParams, value, val => action.Invoke(obj, val.InvariantToString()), updater);
                        }
                    }
                }
            }
        }

        protected static NotifyUpdater GetOrCreateNotifyUpdater(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            NotifyUpdater updater = null;

            if (parserParams.host is System.ComponentModel.INotifyPropertyChanged notifyHost && !componentType.component.gameObject.TryGetComponent(out updater))
            {
                updater = componentType.component.gameObject.AddComponent<NotifyUpdater>();
                updater.NotifyHost = notifyHost;
            }

            return updater;
        }

        protected static NotifyUpdater BindValue(ComponentTypeWithData componentType, BSMLParserParams parserParams, BSMLValue value, Action<object> onChange, NotifyUpdater notifyUpdater = null)
        {
            if (value == null)
            {
                return notifyUpdater;
            }

            if (value is BSMLPropertyValue prop)
            {
                if (notifyUpdater == null)
                {
                    notifyUpdater = GetOrCreateNotifyUpdater(componentType, parserParams);
                }

                if (notifyUpdater != null)
                {
                    notifyUpdater.AddAction(prop.PropertyInfo.Name, onChange);
                }
            }

            return notifyUpdater;
        }
    }

    public class ComponentHandler : Attribute
    {
        public Type type;

        public ComponentHandler(Type type)
        {
            this.type = type;
        }
    }
}
