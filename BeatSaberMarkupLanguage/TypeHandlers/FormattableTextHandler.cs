using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(FormattableText))]
    public class FormattableTextHandler : TypeHandler<FormattableText>
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "data", new[] { "data" } },
            { "dataFormat", new[] { "data-format" } },
            { "dataFormatter", new[] { "data-formatter" } },
        };

        public override Dictionary<string, Action<FormattableText, string>> Setters { get; } = new()
        {
            { "dataFormat", new Action<FormattableText, string>((formattableText, value) => formattableText.TextFormat = value) },
        };

        public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            base.HandleType(componentType, parserParams);
            FormattableText formattableText = componentType.component as FormattableText;
            NotifyUpdater updater = null;
            if (componentType.data.TryGetValue("data", out string dataStr))
            {
                if (parserParams.values.TryGetValue(dataStr, out BSMLValue dataValue))
                {
                    formattableText.Data = dataValue.GetValue();
                    BindValue(componentType, parserParams, dataValue, val => formattableText.Data = val, updater);
                }
                else
                {
                    throw new ValueNotFoundException(dataStr, parserParams.host);
                }
            }

            if (componentType.data.TryGetValue("dataFormatter", out string formatterStr))
            {
                if (parserParams.values.TryGetValue(formatterStr, out BSMLValue formatterValue))
                {
                    formattableText.SetFormatter(formatterValue.GetValue());
                    updater = BindValue(componentType, parserParams, formatterValue, val => formattableText.SetFormatter(val), updater);
                }
                else
                {
                    throw new ValueNotFoundException(formatterStr, parserParams.host);
                }
            }
        }
    }
}
