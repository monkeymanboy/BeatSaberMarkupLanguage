using System.Linq;
using TMPro;
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

            TextMeshProUGUI textMesh = gameObj.AddComponent<TextMeshProUGUI>();
            textMesh.font = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<TMP_FontAsset>().First(t => t.name == "Teko-Medium SDF No Glow"));
            textMesh.fontSize = 4;
            textMesh.color = Color.white;

            textMesh.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            return gameObj;
        }
    }
}
