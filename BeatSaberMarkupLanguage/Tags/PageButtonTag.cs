using BeatSaberMarkupLanguage.Components;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class PageButtonTag : BSMLTag
    {
        private NoTransitionsButton buttonTemplate;

        public override string[] Aliases => new[] { "page-button", "pg-button" };

        public override GameObject CreateObject(Transform parent)
        {
            if (buttonTemplate == null)
            {
                buttonTemplate = (NoTransitionsButton)DiContainer.Resolve<PlayerOptionsViewController>()._playerSettingsPanelController.GetComponent<ScrollView>()._pageUpButton;
            }

            NoTransitionsButton button = Object.Instantiate(buttonTemplate, parent, false);
            button.name = "BSMLPageButton";
            button.interactable = true;

            GameObject gameObject = button.gameObject;
            gameObject.SetActive(false);

            gameObject.AddComponent<PageButton>();
            LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = -1;
            layoutElement.preferredHeight = -1;
            layoutElement.flexibleHeight = 0;
            layoutElement.flexibleWidth = 0;

            ContentSizeFitter sizeFitter = gameObject.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            RectTransform buttonTransform = button.transform.GetChild(0) as RectTransform;
            buttonTransform.anchorMin = new Vector2(0, 0);
            buttonTransform.anchorMax = new Vector2(1, 1);
            buttonTransform.sizeDelta = new Vector2(0, 0);

            (button.transform as RectTransform).pivot = new Vector2(.5f, .5f);

            ImageView image = button.transform.Find("Icon").GetComponent<ImageView>();

            ButtonIconImage btnIcon = gameObject.AddComponent<ButtonIconImage>();
            btnIcon.button = button;
            btnIcon.defaultSkew = image.skew;
            btnIcon.image = image;

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
