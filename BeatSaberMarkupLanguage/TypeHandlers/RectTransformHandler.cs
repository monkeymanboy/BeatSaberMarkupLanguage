using System;
using System.Collections.Generic;
using HMUI;
using Polyglot;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(RectTransform))]
    public class RectTransformHandler : TypeHandler<RectTransform>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "anchorMinX", new[] { "anchor-min-x" } },
            { "anchorMinY", new[] { "anchor-min-y" } },
            { "anchorMaxX", new[] { "anchor-max-x" } },
            { "anchorMaxY", new[] { "anchor-max-y" } },
            { "anchorPosX", new[] { "anchor-pos-x" } },
            { "anchorPosY", new[] { "anchor-pos-y" } },
            { "sizeDeltaX", new[] { "size-delta-x" } },
            { "sizeDeltaY", new[] { "size-delta-y" } },
            { "pivotX", new[] { "pivot-x" } },
            { "pivotY", new[] { "pivot-y" } },
            { "hoverHint", new[] { "hover-hint" } },
            { "hoverHintKey", new[] { "hover-hint-key" } },
            { "active", new[] { "active" } },
        };

        public override Dictionary<string, Action<RectTransform, string>> Setters => new Dictionary<string, Action<RectTransform, string>>()
        {
            { "anchorMinX", new Action<RectTransform, string>((component, value) => component.anchorMin = new Vector2(Parse.Float(value), component.anchorMin.y)) },
            { "anchorMinY", new Action<RectTransform, string>((component, value) => component.anchorMin = new Vector2(component.anchorMin.x, Parse.Float(value))) },
            { "anchorMaxX", new Action<RectTransform, string>((component, value) => component.anchorMax = new Vector2(Parse.Float(value), component.anchorMax.y)) },
            { "anchorMaxY", new Action<RectTransform, string>((component, value) => component.anchorMax = new Vector2(component.anchorMax.x, Parse.Float(value))) },
            { "anchorPosX", new Action<RectTransform, string>((component, value) => component.anchoredPosition = new Vector2(Parse.Float(value), component.anchoredPosition.y)) },
            { "anchorPosY", new Action<RectTransform, string>((component, value) => component.anchoredPosition = new Vector2(component.anchoredPosition.x, Parse.Float(value))) },
            { "sizeDeltaX", new Action<RectTransform, string>((component, value) => component.sizeDelta = new Vector2(Parse.Float(value), component.sizeDelta.y)) },
            { "sizeDeltaY", new Action<RectTransform, string>((component, value) => component.sizeDelta = new Vector2(component.sizeDelta.x, Parse.Float(value))) },
            { "pivotX", new Action<RectTransform, string>((component, value) => component.pivot = new Vector2(Parse.Float(value), component.pivot.y)) },
            { "pivotY", new Action<RectTransform, string>((component, value) => component.pivot = new Vector2(component.pivot.x, Parse.Float(value))) },
            { "hoverHint", new Action<RectTransform, string>(AddHoverHint) },
            { "hoverHintKey", new Action<RectTransform, string>(AddHoverHintKey) },
            { "active", new Action<RectTransform, string>((component, value) => component.gameObject.SetActive(Parse.Bool(value))) },
        };

        private void AddHoverHint(RectTransform rectTransform, string text)
        {
            HoverHint hover = BeatSaberUI.DiContainer.InstantiateComponent<HoverHint>(rectTransform.gameObject);
            hover.text = text;
        }

        private void AddHoverHintKey(RectTransform rectTransform, string key)
        {
            AddHoverHint(rectTransform, Localization.Get(key));
        }
    }
}
