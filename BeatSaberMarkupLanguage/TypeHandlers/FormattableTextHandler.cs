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

                if (componentType.valueMap != null)
                {
                    dataValue = componentType.valueMap.TryGetValue("data", out BSMLValue existingValue)
                        ? existingValue : null;
                    formatterValue = componentType.valueMap.TryGetValue("data-formatter", out existingValue)
                        ? existingValue : null;
                }

                //-----data-----
                if (dataValue != null)
                {
                    formattableText.Data = dataValue.GetValue();
                    if (dataValue is BSMLPropertyValue dataProp)
                    {
                        if (updater == null)
                            updater = GetOrCreateNotifyUpdater(componentType, parserParams);
                        updater.AddAction(dataProp.propertyInfo.Name, val => formattableText.Data = val);
                    }
                }
                else if (componentType.data.TryGetValue("data", out string dataStr))
                {
                    if (parserParams.values.TryGetValue(dataStr, out dataValue))
                        formattableText.Data = dataValue.GetValue();
                    else
                        formattableText.Data = dataStr;
                }

                //-----data-formatter-----
                if (formatterValue != null)
                {
                    formattableText.SetFormatter(formatterValue.GetValue());
                    if (formatterValue is BSMLPropertyValue formatterProp)
                    {
                        if (updater == null)
                            updater = GetOrCreateNotifyUpdater(componentType, parserParams);
                        updater.AddAction(formatterProp.propertyInfo.Name, val => formattableText.SetFormatter(val));
                    }
                }
                else if (componentType.data.TryGetValue("data-formatter", out string formatterStr))
                {
                    throw new InvalidOperationException($"'data-formatter' value of '{formatterStr}' is invalid in {parserParams.host?.GetType().FullName}. UIValue name must be prefixed with '{BSMLParser.RETRIEVE_VALUE_PREFIX}'.");
                }
            }

        }
    }
}
