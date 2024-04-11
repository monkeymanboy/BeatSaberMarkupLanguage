using System;
using System.Collections.Generic;
using BGLib.Polyglot;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(RectTransform))]
    public class RectTransformHandler : TypeHandler<RectTransform>
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "active", new[] { "active" } },
            { "anchorMin", new[] { "anchor-min" } },
            { "anchorMinX", new[] { "anchor-min-x" } },
            { "anchorMinY", new[] { "anchor-min-y" } },
            { "anchorMax", new[] { "anchor-max" } },
            { "anchorMaxX", new[] { "anchor-max-x" } },
            { "anchorMaxY", new[] { "anchor-max-y" } },
            { "anchorPos", new[] { "anchored-position" } },
            { "anchorPosX", new[] { "anchored-position-x", "anchor-pos-x" } },
            { "anchorPosY", new[] { "anchored-position-y", "anchor-pos-y" } },
            { "hoverHint", new[] { "hover-hint" } },
            { "hoverHintKey", new[] { "hover-hint-key" } },
            { "localScale", new[] { "local-scale", "scale" } },
            { "name", new[] { "name" } },
            { "pivot", new[] { "pivot" } },
            { "pivotX", new[] { "pivot-x" } },
            { "pivotY", new[] { "pivot-y" } },
            { "sizeDelta", new[] { "size-delta" } },
            { "sizeDeltaX", new[] { "size-delta-x" } },
            { "sizeDeltaY", new[] { "size-delta-y" } },
        };

        public override Dictionary<string, Action<RectTransform, string>> Setters => new()
        {
            { "active", new Action<RectTransform, string>((component, value) => component.gameObject.SetActive(Parse.Bool(value))) },
            { "anchorMin", new Action<RectTransform, string>((component, value) => component.anchorMin = Parse.Vector2(value)) },
            { "anchorMinX", new Action<RectTransform, string>((component, value) => component.anchorMin = new Vector2(Parse.Float(value), component.anchorMin.y)) },
            { "anchorMinY", new Action<RectTransform, string>((component, value) => component.anchorMin = new Vector2(component.anchorMin.x, Parse.Float(value))) },
            { "anchorMax", new Action<RectTransform, string>((component, value) => component.anchorMax = Parse.Vector2(value)) },
            { "anchorMaxX", new Action<RectTransform, string>((component, value) => component.anchorMax = new Vector2(Parse.Float(value), component.anchorMax.y)) },
            { "anchorMaxY", new Action<RectTransform, string>((component, value) => component.anchorMax = new Vector2(component.anchorMax.x, Parse.Float(value))) },
            { "anchorPos", new Action<RectTransform, string>((component, value) => component.anchoredPosition = Parse.Vector2(value)) },
            { "anchorPosX", new Action<RectTransform, string>((component, value) => component.anchoredPosition = new Vector2(Parse.Float(value), component.anchoredPosition.y)) },
            { "anchorPosY", new Action<RectTransform, string>((component, value) => component.anchoredPosition = new Vector2(component.anchoredPosition.x, Parse.Float(value))) },
            { "hoverHint", new Action<RectTransform, string>(AddHoverHint) },
            { "hoverHintKey", new Action<RectTransform, string>(AddHoverHintKey) },
            { "localScale", new Action<RectTransform, string>((component, value) => component.localScale = Parse.Vector3(value, 1)) },
            { "name", new Action<RectTransform, string>((component, value) => component.name = value) },
            { "pivot", new Action<RectTransform, string>((component, value) => component.pivot = Parse.Vector2(value)) },
            { "pivotX", new Action<RectTransform, string>((component, value) => component.pivot = new Vector2(Parse.Float(value), component.pivot.y)) },
            { "pivotY", new Action<RectTransform, string>((component, value) => component.pivot = new Vector2(component.pivot.x, Parse.Float(value))) },
            { "sizeDelta", new Action<RectTransform, string>((component, value) => component.sizeDelta = Parse.Vector2(value)) },
            { "sizeDeltaX", new Action<RectTransform, string>((component, value) => component.sizeDelta = new Vector2(Parse.Float(value), component.sizeDelta.y)) },
            { "sizeDeltaY", new Action<RectTransform, string>((component, value) => component.sizeDelta = new Vector2(component.sizeDelta.x, Parse.Float(value))) },
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
