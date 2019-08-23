using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            { "sizeDeltaY", new[]{ "size-delta-y" } }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, Dictionary<string, Action> actions)
        {
            RectTransform rectTransform = obj as RectTransform;
            rectTransform.anchorMin = new Vector2(data.ContainsKey("anchorMinX") ? float.Parse(data["anchorMinX"]) : rectTransform.anchorMin.x, data.ContainsKey("anchorMinY") ? float.Parse(data["anchorMinY"]) : rectTransform.anchorMin.y);
            rectTransform.anchorMax = new Vector2(data.ContainsKey("anchorMaxX") ? float.Parse(data["anchorMaxX"]) : rectTransform.anchorMax.x, data.ContainsKey("anchorMaxY") ? float.Parse(data["anchorMaxY"]) : rectTransform.anchorMax.y);
            rectTransform.anchoredPosition = new Vector2(data.ContainsKey("anchorPosX") ? float.Parse(data["anchorPosX"]) : rectTransform.anchoredPosition.x, data.ContainsKey("anchorPosY") ? float.Parse(data["anchorPosY"]) : rectTransform.anchoredPosition.y);
            rectTransform.sizeDelta = new Vector2(data.ContainsKey("sizeDeltaX") ? float.Parse(data["sizeDeltaX"]) : rectTransform.sizeDelta.x, data.ContainsKey("sizeDeltaY") ? float.Parse(data["sizeDeltaY"]) : rectTransform.sizeDelta.y);
        }
    }
}
