using System.Linq;
using BeatSaberMarkupLanguage.Components;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ButtonWithIconTag : BSMLTag
    {
        private Button buttonWithIconTemplate;

        public override string[] Aliases => new[] { "button-with-icon", "icon-button" };

        public override GameObject CreateObject(Transform parent)
        {
            if (buttonWithIconTemplate == null)
            {
                buttonWithIconTemplate = BeatSaberUI.DiContainer.Resolve<StandardLevelDetailViewController>()._standardLevelDetailView.practiceButton;
            }

            Button button = Object.Instantiate(buttonWithIconTemplate, parent, false);
            button.name = "BSMLIconButton";
            button.interactable = true;

            Object.Destroy(button.GetComponent<HoverHint>());
            Object.Destroy(button.GetComponent<LocalizedHoverHint>());
            button.gameObject.AddComponent<ExternalComponents>().components.Add(button.GetComponentsInChildren<LayoutGroup>().Where(x => x.name == "Content").First());

            Transform contentTransform = button.transform.Find("Content");
            Object.Destroy(contentTransform.Find("Text").gameObject);
            Image iconImage = new GameObject("Icon").AddComponent<ImageView>();
            iconImage.material = Utilities.ImageResources.NoGlowMat;
            iconImage.rectTransform.SetParent(contentTransform, false);
            iconImage.rectTransform.sizeDelta = new Vector2(20f, 20f);
            iconImage.sprite = Utilities.ImageResources.BlankSprite;
            iconImage.preserveAspect = true;
            if (iconImage != null)
            {
                ButtonIconImage btnIcon = button.gameObject.AddComponent<ButtonIconImage>();
                btnIcon.image = iconImage;
            }

            Object.Destroy(button.transform.Find("Content").GetComponent<LayoutElement>());

            ContentSizeFitter buttonSizeFitter = button.gameObject.AddComponent<ContentSizeFitter>();
            buttonSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            buttonSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            return button.gameObject;
        }
    }
}
