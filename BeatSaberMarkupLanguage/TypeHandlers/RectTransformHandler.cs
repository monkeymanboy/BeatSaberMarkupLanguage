using BeatSaberMarkupLanguage.Parser;
using BS_Utils.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            RectTransform rectTransform = obj as RectTransform;
            rectTransform.anchorMin = new Vector2(data.ContainsKey("anchorMinX") ? Parse.Float(data["anchorMinX"]) : rectTransform.anchorMin.x, data.ContainsKey("anchorMinY") ? Parse.Float(data["anchorMinY"]) : rectTransform.anchorMin.y);
            rectTransform.anchorMax = new Vector2(data.ContainsKey("anchorMaxX") ? Parse.Float(data["anchorMaxX"]) : rectTransform.anchorMax.x, data.ContainsKey("anchorMaxY") ? Parse.Float(data["anchorMaxY"]) : rectTransform.anchorMax.y);
            rectTransform.anchoredPosition = new Vector2(data.ContainsKey("anchorPosX") ? Parse.Float(data["anchorPosX"]) : rectTransform.anchoredPosition.x, data.ContainsKey("anchorPosY") ? Parse.Float(data["anchorPosY"]) : rectTransform.anchoredPosition.y);
            rectTransform.sizeDelta = new Vector2(data.ContainsKey("sizeDeltaX") ? Parse.Float(data["sizeDeltaX"]) : rectTransform.sizeDelta.x, data.ContainsKey("sizeDeltaY") ? Parse.Float(data["sizeDeltaY"]) : rectTransform.sizeDelta.y);
            if (data.ContainsKey("hoverHint"))
            {
                HoverHint hover = obj.gameObject.AddComponent<HoverHint>();
                hover.text = data["hoverHint"];
                hover.SetPrivateField("_hoverHintController", Resources.FindObjectsOfTypeAll<HoverHintController>().First());
            }
        }
    }
}
