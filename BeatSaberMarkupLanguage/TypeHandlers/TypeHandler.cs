using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Notify;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using UnityEngine;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    public abstract class TypeHandler
    {
        private Dictionary<string, string[]> cachedProps;
        public Dictionary<string, string[]> CachedProps
        {
            get{
                if (cachedProps == null)
                    cachedProps = Props;
                return cachedProps;
            }
        }
        public abstract Dictionary<string, string[]> Props { get; }
        public abstract void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams);
        public virtual void HandleTypeAfterChildren(ComponentTypeWithData componentType, BSMLParserParams parserParams) { }
        public virtual void HandleTypeAfterParse(ComponentTypeWithData componentType, BSMLParserParams parserParams) { }
    }

    public abstract class TypeHandler<T> : TypeHandler
        where T : Component
    {

        private Dictionary<string, Action<T, string>> cachedSetters;
        public Dictionary<string, Action<T, string>> CachedSetters
        {
            get
            {
                if (cachedSetters == null)
                    cachedSetters = Setters;
                return cachedSetters;
            }
        }
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
                            updater = BindValue(componentType, parserParams, value, val => action.Invoke(obj, val.InvariantToString()), updater);
                    }
                }
            }
        }
        protected static NotifyUpdater GetOrCreateNotifyUpdater(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            NotifyUpdater updater = null;
            if (parserParams.host is System.ComponentModel.INotifyPropertyChanged notifyHost)
            {
                updater = componentType.component.gameObject.GetComponent<NotifyUpdater>();
                if (updater == null)
                {
                    updater = componentType.component.gameObject.AddComponent<NotifyUpdater>();
                    updater.NotifyHost = notifyHost;
                }
            }
            return updater;
        }

        protected static NotifyUpdater BindValue(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams, BSMLValue value, Action<object> onChange, NotifyUpdater notifyUpdater = null)
        {
            if (value == null) return notifyUpdater;
            if (value is BSMLPropertyValue prop)
            {
                if (notifyUpdater == null)
                    notifyUpdater = GetOrCreateNotifyUpdater(componentType, parserParams);
                notifyUpdater?.AddAction(prop.propertyInfo.Name, onChange);
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
