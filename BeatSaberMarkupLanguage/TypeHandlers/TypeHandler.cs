using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Notify;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    public abstract class TypeHandler
    {
        public abstract Dictionary<string, string[]> Props { get; }
        public virtual void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams) { }
        public virtual void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams, Dictionary<string, PropertyInfo> propertyMap)
        {
            HandleType(obj, data, parserParams);
        }
        public virtual void HandleTypeAfterChildren(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams, Dictionary<string, PropertyInfo> propertyMap = null) { }
    }

    public abstract class TypeHandler<T> : TypeHandler
        where T : Component
    {
        public abstract Dictionary<string, Action<T, string>> Setters { get; }
        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            if (obj is T castObj)
                HandleType(castObj, data, parserParams, null);
        }

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams, Dictionary<string, PropertyInfo> propertyMap)
        {
            if (obj is T castObj)
                HandleType(castObj, data, parserParams, propertyMap);
        }

        public virtual void HandleType(T obj, Dictionary<string, string> data, BSMLParserParams parserParams, Dictionary<string, PropertyInfo> propertyMap = null)
        {
            NotifyUpdater updater = obj.gameObject.GetComponent<NotifyUpdater>() ?? obj.gameObject.GetComponent<NotifyUpdater>();
            foreach (KeyValuePair<string, string> pair in data)
            {
                if (Setters.TryGetValue(pair.Key, out Action<T, string> action))
                {
                    action.Invoke(obj, pair.Value);
                    if (updater != null && propertyMap != null && propertyMap.TryGetValue(pair.Key, out PropertyInfo prop))
                    {
                        Logger.log?.Warn($"Mapping {pair.Key} to property {prop.Name}");
                        updater?.AddAction(prop.Name, val => action.Invoke(obj, val.ToString()));
                    }
                }
                else
                    Logger.log?.Warn($"Tag {pair.Key} not supported for {obj.GetType().Name}");
            }
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
