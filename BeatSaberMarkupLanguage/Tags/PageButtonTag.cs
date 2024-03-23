using System.Linq;
using BeatSaberMarkupLanguage.Components;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class PageButtonTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "page-button", "pg-button" };

        protected override PrefabParams CreatePrefab()
        {
            Button button = Object.Instantiate(BeatSaberUI.DiContainer.Resolve<PlayerOptionsViewController>()._playerSettingsPanelController.GetComponent<ScrollView>()._pageUpButton);
            GameObject buttonObject = button.gameObject;
            buttonObject.SetActive(false);
            button.name = "BSMLPageButton";
            button.interactable = true;
            buttonObject.AddComponent<PageButton>();
            LayoutElement layoutElement = buttonObject.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = -1;
            layoutElement.preferredHeight = -1;
            layoutElement.flexibleHeight = 0;
            layoutElement.flexibleWidth = 0;

            ContentSizeFitter sizeFitter = buttonObject.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            RectTransform buttonTransform = (RectTransform)button.transform.GetChild(0);
            buttonTransform.anchorMin = new Vector2(0, 0);
            buttonTransform.anchorMax = new Vector2(1, 1);
            buttonTransform.sizeDelta = new Vector2(0, 0);

            ((RectTransform)button.transform).pivot = new Vector2(.5f, .5f);

            ButtonIconImage btnIcon = buttonObject.AddComponent<ButtonIconImage>();
            btnIcon.image = buttonObject.GetComponentsInChildren<Image>(true).Where(x => x.gameObject.name == "Icon").FirstOrDefault();

            return new PrefabParams(buttonObject);
        }
    }
}
