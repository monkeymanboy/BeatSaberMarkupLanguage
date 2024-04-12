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
            { "sprite", new[] { "sprite" } },
        };

        public override Dictionary<string, Action<ButtonIconImage, string>> Setters => new()
        {
            { "icon", new Action<ButtonIconImage, string>((image, value) => image.SetIcon(value)) },
            { "iconSkew", new Action<ButtonIconImage, string>((image, value) => image.SetSkew(Parse.Float(value))) },
            { "showUnderline", new Action<ButtonIconImage, string>((image, value) => image.SetUnderlineActive(Parse.Bool(value))) },
        };

        public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            base.HandleType(componentType, parserParams);

            ButtonIconImage buttonIconImage = (ButtonIconImage)componentType.component;

            if (componentType.data.TryGetValue("sprite", out string sprite))
            {
                if (!parserParams.values.TryGetValue(sprite, out BSMLValue value))
                {
                    throw new ValueNotFoundException(sprite, parserParams.host);
                }

                buttonIconImage.image.sprite = value.GetValueAs<Sprite>();
            }
        }
    }
}
