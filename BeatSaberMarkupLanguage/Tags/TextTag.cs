using BeatSaberMarkupLanguage.Components;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class TextTag : BSMLTag
    {
        public override string[] Aliases => new[] { "text", "label" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObj = new GameObject("BSMLText");
            gameObj.transform.SetParent(parent, false);

            FormattableText textMesh = gameObj.AddComponent<FormattableText>();
            textMesh.font = Object.Instantiate(BeatSaberUI.MainTextFont);
            textMesh.fontSharedMaterial = BeatSaberUI.MainUIFontMaterial;
            textMesh.fontSize = 4;
            textMesh.color = Color.white;
            textMesh.text = "Default Text";

            textMesh.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            CreateLocalizableText(gameObj);
            return gameObj;
        }
    }
}