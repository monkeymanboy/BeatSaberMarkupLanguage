using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Exceptions;
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
            if (componentType.Component is not T obj)
            {
                return;
            }

            NotifyUpdater updater = null;

            foreach (KeyValuePair<string, string> pair in componentType.Data)
            {
                if (!CachedSetters.TryGetValue(pair.Key, out Action<T, string> action))
                {
                    continue;
                }

                try
                {
                    action.Invoke(obj, pair.Value);
                }
                catch (Exception ex)
                {
                    throw new TypeHandlerException(this, pair.Key, ex);
                }

                if (componentType.ValueMap.TryGetValue(pair.Key, out BSMLValue value))
                {
                    updater = BindValue(componentType, parserParams, value, val => action.Invoke(obj, val.InvariantToString()), updater);
                }
            }
        }

        protected static NotifyUpdater GetOrCreateNotifyUpdater(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            NotifyUpdater updater = null;

            if (parserParams.Host is System.ComponentModel.INotifyPropertyChanged notifyHost && !componentType.Component.gameObject.TryGetComponent(out updater))
            {
                updater = componentType.Component.gameObject.AddComponent<NotifyUpdater>();
                updater.NotifyHost = notifyHost;
            }

            return updater;
        }

        protected static NotifyUpdater BindValue(ComponentTypeWithData componentType, BSMLParserParams parserParams, BSMLValue value, Action<object> onChange, NotifyUpdater notifyUpdater = null)
        {
            if (value is not BSMLPropertyValue prop)
            {
                return notifyUpdater;
            }

            if (notifyUpdater == null)
            {
                notifyUpdater = GetOrCreateNotifyUpdater(componentType, parserParams);
            }

            if (notifyUpdater != null)
            {
                notifyUpdater.AddAction(prop.PropertyInfo.Name, onChange);
            }

            return notifyUpdater;
        }
    }

    public class ComponentHandler : Attribute
    {
        public ComponentHandler(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; }
    }
}
