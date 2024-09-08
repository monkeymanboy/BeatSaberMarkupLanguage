﻿using System;
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
            FormattableText formattableText = componentType.Component as FormattableText;

            if (componentType.Data.TryGetValue("data", out string dataStr))
            {
                if (parserParams.Values.TryGetValue(dataStr, out BSMLValue dataValue))
                {
                    formattableText.Data = dataValue.GetValue();
                    BindValue(componentType, parserParams, dataValue, val => formattableText.Data = val);
                }
                else
                {
                    throw new ValueNotFoundException(dataStr, parserParams.Host);
                }
            }

            if (componentType.Data.TryGetValue("dataFormatter", out string formatterStr))
            {
                if (parserParams.Values.TryGetValue(formatterStr, out BSMLValue formatterValue))
                {
                    formattableText.SetFormatter(formatterValue.GetValue());
                    BindValue(componentType, parserParams, formatterValue, val => formattableText.SetFormatter(val));
                }
                else
                {
                    throw new ValueNotFoundException(formatterStr, parserParams.Host);
                }
            }
        }
    }
}
