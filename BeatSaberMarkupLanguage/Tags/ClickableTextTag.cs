using BeatSaberMarkupLanguage.Components;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ClickableTextTag : BSMLTag
    {
        public override string[] Aliases => new[] { "clickable-text" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = new GameObject();
            gameObject.name = "BSMLClickableText";
            gameObject.SetActive(false);

            ClickableText clickableText = gameObject.AddComponent<ClickableText>();
            clickableText.font = GameObject.Instantiate(Resources.FindObjectsOfTypeAll<TMP_FontAsset>().First(t => t.name == "Teko-Medium SDF No Glow"));
            clickableText.rectTransform.SetParent(parent, false);
            clickableText.text = "Default Text";
            clickableText.fontSize = 5;
            clickableText.color = Color.white;
            clickableText.rectTransform.sizeDelta = new Vector2(90, 8);

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
