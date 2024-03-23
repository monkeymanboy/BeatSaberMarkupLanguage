using BeatSaberMarkupLanguage.Components;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class TextTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "text", "label" };

        protected override PrefabParams CreatePrefab()
        {
            GameObject gameObj = new("BSMLText");
            gameObj.SetActive(false);

            FormattableText textMesh = gameObj.AddComponent<FormattableText>();
            textMesh.font = BeatSaberUI.MainTextFont;
            textMesh.fontSharedMaterial = BeatSaberUI.MainUIFontMaterial;
            textMesh.fontSize = 4;
            textMesh.color = Color.white;
            textMesh.text = "Default Text";

            textMesh.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            CreateLocalizableText(gameObj);
            return new PrefabParams(gameObj);
        }
    }
}
