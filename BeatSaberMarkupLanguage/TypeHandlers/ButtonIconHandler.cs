using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(ButtonIconImage))]
    public class ButtonIconHandler : TypeHandler<ButtonIconImage>
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "icon", new[] { "icon" } },
            { "iconSkew", new[] { "icon-skew" } },
            { "showUnderline", new[] { "show-underline" } },
        };

        public override Dictionary<string, Action<ButtonIconImage, string>> Setters => new()
        {
            { "iconSkew", new Action<ButtonIconImage, string>((image, value) => image.SetSkew(Parse.Float(value))) },
            { "showUnderline", new Action<ButtonIconImage, string>((image, value) => image.SetUnderlineActive(Parse.Bool(value))) },
        };

        public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            ButtonIconImage buttonIconImage = (ButtonIconImage)componentType.Component;

            if (componentType.ValueMap.TryGetValue("icon", out BSMLValue value))
            {
                SetIcon(buttonIconImage, value.GetValue());
                BindValue(componentType, parserParams, value, val => SetIcon(buttonIconImage, val));
            }
            else if (componentType.Data.TryGetValue("icon", out string str))
            {
                buttonIconImage.SetIcon(str);
            }
        }

        private static void SetIcon(ButtonIconImage buttonIconImage, object value)
        {
            if (value is Sprite sprite)
            {
                buttonIconImage.Image.sprite = sprite;
            }
            else
            {
                buttonIconImage.SetIcon(value.InvariantToString());
            }
        }
    }
}
