using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Util;
using Polyglot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ButtonTag : BSMLTag
    {
        private Button buttonPrefab;

        public override string[] Aliases => new[] { "button" };

        public override void Setup()
        {
            base.Setup();
            buttonPrefab = GetButtonPrefab();
        }

        public override GameObject CreateObject(Transform parent)
        {
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

        protected virtual Button GetButtonPrefab()
        {
            return DiContainer.Resolve<StandardLevelDetailViewController>().GetComponentOnChild<Button>("LevelDetail/ActionButtons/PracticeButton");
        }
    }
}
