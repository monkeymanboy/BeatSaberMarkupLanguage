using BeatSaberMarkupLanguage.Parser;
using BS_Utils.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(RectTransform))]
    public class RectTransformHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "anchorMinX", new[]{ "anchor-min-x" } },
            { "anchorMinY", new[]{ "anchor-min-y" } },
            { "anchorMaxX", new[]{ "anchor-max-x" } },
            { "anchorMaxY", new[]{ "anchor-max-y" } },
            { "anchorPosX", new[]{ "anchor-pos-x" } },
            { "anchorPosY", new[]{ "anchor-pos-y" } },
            { "sizeDeltaX", new[]{ "size-delta-x" } },
            { "sizeDeltaY", new[]{ "size-delta-y" } },
            { "hoverHint", new[]{ "hover-hint" } }
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            RectTransform rectTransform = componentType.component as RectTransform;
            rectTransform.anchorMin = new Vector2(
                componentType.data.TryGetValue("anchorMinX", out string anchorMinX)
                    ? Parse.Float(anchorMinX)
                    : rectTransform.anchorMin.x,
                componentType.data.TryGetValue("anchorMinY", out string anchorMinY)
                    ? Parse.Float(anchorMinY)
                    : rectTransform.anchorMin.y);

            rectTransform.anchorMax = new Vector2(
                componentType.data.TryGetValue("anchorMaxX", out string anchorMaxX) 
                    ? Parse.Float(anchorMaxX) 
                    : rectTransform.anchorMax.x, 
                componentType.data.TryGetValue("anchorMaxY", out string anchorMaxY) 
                    ? Parse.Float(anchorMaxY) 
                    : rectTransform.anchorMax.y);

            rectTransform.anchoredPosition = new Vector2(
                componentType.data.TryGetValue("anchorPosX", out string anchorPosX) 
                    ? Parse.Float(anchorPosX) 
                    : rectTransform.anchoredPosition.x, 
                componentType.data.TryGetValue("anchorPosY", out string anchorPosY) 
                    ? Parse.Float(anchorPosY) 
                    : rectTransform.anchoredPosition.y);

            rectTransform.sizeDelta = new Vector2(
                componentType.data.TryGetValue("sizeDeltaX", out string sizeDeltaX) 
                    ? Parse.Float(sizeDeltaX) 
                    : rectTransform.sizeDelta.x, 
                componentType.data.TryGetValue("sizeDeltaY", out string sizeDeltaY) 
                    ? Parse.Float(sizeDeltaY) 
                    : rectTransform.sizeDelta.y);

            if (componentType.data.TryGetValue("hoverHint", out string hoverHint))
            {
                HoverHint hover = componentType.component.gameObject.AddComponent<HoverHint>();
                hover.text = hoverHint;
                hover.SetPrivateField("_hoverHintController", Resources.FindObjectsOfTypeAll<HoverHintController>().First());
            }
        }
    }
}
