using System.Linq;
using BeatSaberMarkupLanguage.Components;
using Polyglot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ButtonTag : BSMLTag
    {
        public override string[] Aliases => new[] { "button" };

        public virtual string PrefabButton => "PracticeButton";

        private Button buttonPrefab;

        public override GameObject CreateObject(Transform parent)
        {
            if (buttonPrefab == null)
            {
                buttonPrefab = Resources.FindObjectsOfTypeAll<Button>().Last(x => (x.name == PrefabButton));
            }

            Button button = Object.Instantiate(buttonPrefab, parent, false);
            button.name = "BSMLButton";
            button.interactable = true;

            GameObject gameObject = button.gameObject;
            gameObject.SetActive(true);

            ExternalComponents externalComponents = gameObject.AddComponent<ExternalComponents>();
            GameObject textObject = button.transform.Find("Content/Text").gameObject;

            LocalizedTextMeshProUGUI localizedText = ConfigureLocalizedText(textObject);
            externalComponents.components.Add(localizedText);

            TextMeshProUGUI textMesh = textObject.GetComponent<TextMeshProUGUI>();
            textMesh.text = "Default Text";
            textMesh.richText = true;
            externalComponents.components.Add(textMesh);

            Object.Destroy(button.transform.Find("Content").GetComponent<LayoutElement>());

            ContentSizeFitter buttonSizeFitter = gameObject.AddComponent<ContentSizeFitter>();
            buttonSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            buttonSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            LayoutGroup stackLayoutGroup = button.GetComponentInChildren<LayoutGroup>();
            if (stackLayoutGroup != null)
            {
                externalComponents.components.Add(stackLayoutGroup);
            }

            /*
            Image glowImage = gameObject.GetComponentsInChildren<Image>(true).Where(x => x.gameObject.name == "Glow").FirstOrDefault();
            if (glowImage != null)
            {
                Glowable glowable = gameObject.AddComponent<Glowable>();
                glowable.image = glowImage;
                glowable.SetGlow("none");
            }

            Image strokeImage = gameObject.GetComponentsInChildren<Image>(true).Where(x => x.gameObject.name == "Stroke").FirstOrDefault();
            if (strokeImage != null)
            {
                Strokable strokable = gameObject.AddComponent<Strokable>();
                strokable.image = strokeImage;
                strokable.SetType(StrokeType.Regular);
            }*/

            return gameObject;
        }
    }
}
