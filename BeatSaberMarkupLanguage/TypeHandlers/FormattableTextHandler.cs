using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Notify;
using BeatSaberMarkupLanguage.Parser;
using IPA.Config.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(FormattableText))]
    public class FormattableTextHandler : TypeHandler<FormattableText>
    {

        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "data", new[]{ "data" } },
            { "data-format", new[]{ "data-format" } },
            { "data-formatter", new[]{ "data-formatter" } }
        };

        public override Dictionary<string, Action<FormattableText, string>> Setters { get; } = new Dictionary<string, Action<FormattableText, string>>()
        {
            {"data-format", new Action<FormattableText,string>((formattableText, value) => formattableText.TextFormat = value) },
        };

        public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            base.HandleType(componentType, parserParams);
            if (componentType.component is FormattableText formattableText)
            {
                NotifyUpdater updater = null;
                BSMLValue dataValue = null;
                BSMLValue formatterValue = null;

                if (componentType.propertyMap != null)
                {
                    dataValue = componentType.propertyMap.TryGetValue("data", out BSMLPropertyValue existingProp) 
                        ? existingProp : null;
                    formatterValue = componentType.propertyMap.TryGetValue("data-formatter", out existingProp) 
                        ? existingProp : null;
                }

                //-----data-----
                if (dataValue != null || (componentType.data.TryGetValue("data", out string dataStr)
                    && parserParams.values.TryGetValue(dataStr, out dataValue)))
                {
                    formattableText.Data = dataValue.GetValue();
                    if (dataValue is BSMLPropertyValue dataProp)
                    {
                        if (updater == null)
                            updater = CreateNotifyUpdater(componentType, parserParams);
                        updater.AddAction(dataProp.propertyInfo.Name, val => formattableText.Data = val);
                    }
                }
                else if (dataStr != null)
                {
                    formattableText.Data = dataStr;
                }

                //-----data-formatter-----
                if (formatterValue != null || (componentType.data.TryGetValue("data-formatter", out string formatterStr)
                    && parserParams.values.TryGetValue(formatterStr, out formatterValue)))
                {
                    formattableText.SetFormatter(formatterValue.GetValue());
                    if (formatterValue is BSMLPropertyValue formatterProp)
                    {
                        if (updater == null)
                            updater = CreateNotifyUpdater(componentType, parserParams);
                        updater.AddAction(formatterProp.propertyInfo.Name, val => formattableText.SetFormatter(val));
                    }
                }
            }

        }

        internal static NotifyUpdater CreateNotifyUpdater(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            NotifyUpdater updater = null;
            if (parserParams.host is INotifiableHost notifyHost && componentType.propertyMap != null)
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
    }
}
