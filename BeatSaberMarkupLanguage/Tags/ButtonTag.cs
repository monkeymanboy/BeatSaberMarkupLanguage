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
        private Button buttonPrefab;

        public override string[] Aliases => new[] { "button" };

        public virtual string PrefabButton => "PracticeButton";

        public override GameObject CreateObject(Transform parent)
        {
            if (buttonPrefab == null)
            {
                buttonPrefab = Resources.FindObjectsOfTypeAll<Button>().Where(x => x.name == PrefabButton).Last();
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

            return gameObject;
        }
    }
}
