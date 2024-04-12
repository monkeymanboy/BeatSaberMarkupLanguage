using System.Linq;
using BeatSaberMarkupLanguage.Components;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ButtonWithIconTag : BSMLTag
    {
        private NoTransitionsButton buttonWithIconTemplate;

        public override string[] Aliases => new[] { "button-with-icon", "icon-button" };

        public override GameObject CreateObject(Transform parent)
        {
            if (buttonWithIconTemplate == null)
            {
                buttonWithIconTemplate = (NoTransitionsButton)BeatSaberUI.DiContainer.Resolve<StandardLevelDetailViewController>()._standardLevelDetailView.practiceButton;
            }

            NoTransitionsButton button = Object.Instantiate(buttonWithIconTemplate, parent, false);
            button.name = "BSMLIconButton";
            button.interactable = true;

            GameObject gameObject = button.gameObject;
            gameObject.SetActive(false);

            Object.Destroy(button.GetComponent<HoverHint>());
            Object.Destroy(button.GetComponent<LocalizedHoverHint>());
            gameObject.AddComponent<ExternalComponents>().components.Add(button.GetComponentsInChildren<LayoutGroup>().Where(x => x.name == "Content").First());

            Transform contentTransform = button.transform.Find("Content");
            Object.Destroy(contentTransform.Find("Text").gameObject);
            Image iconImage = new GameObject("Icon").AddComponent<ImageView>();
            iconImage.material = Utilities.ImageResources.NoGlowMat;
            iconImage.rectTransform.SetParent(contentTransform, false);
            iconImage.rectTransform.sizeDelta = new Vector2(20f, 20f);
            iconImage.sprite = Utilities.ImageResources.BlankSprite;
            iconImage.preserveAspect = true;

            ButtonIconImage btnIcon = gameObject.AddComponent<ButtonIconImage>();
            btnIcon.button = button;
            btnIcon.image = iconImage;
            btnIcon.SetSkew(button.transform.Find("BG").GetComponent<ImageView>().skew);

            Object.Destroy(button.transform.Find("Content").GetComponent<LayoutElement>());

            ContentSizeFitter buttonSizeFitter = gameObject.AddComponent<ContentSizeFitter>();
            buttonSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            buttonSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
