using System.Linq;
using BeatSaberMarkupLanguage.Components;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class PageButtonTag : BSMLTag
    {
        private Button buttonTemplate;

        public override string[] Aliases => new[] { "page-button", "pg-button" };

        public override GameObject CreateObject(Transform parent)
        {
            if (buttonTemplate == null)
            {
                buttonTemplate = Resources.FindObjectsOfTypeAll<Button>().Last(x => x.name == "UpButton");
            }

            Button button = Object.Instantiate(buttonTemplate, parent, false);
            button.gameObject.SetActive(false);
            button.name = "BSMLPageButton";
            button.interactable = true;
            button.gameObject.AddComponent<PageButton>();
            LayoutElement layoutElement = button.gameObject.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = -1;
            layoutElement.preferredHeight = -1;
            layoutElement.flexibleHeight = 0;
            layoutElement.flexibleWidth = 0;

            ContentSizeFitter sizeFitter = button.gameObject.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            RectTransform buttonTransform = button.transform.GetChild(0) as RectTransform;
            buttonTransform.anchorMin = new Vector2(0, 0);
            buttonTransform.anchorMax = new Vector2(1, 1);
            buttonTransform.sizeDelta = new Vector2(0, 0);

            (button.transform as RectTransform).pivot = new Vector2(.5f, .5f);

            ButtonIconImage btnIcon = button.gameObject.AddComponent<ButtonIconImage>();
            btnIcon.image = button.gameObject.GetComponentsInChildren<Image>(true).Where(x => x.gameObject.name == "Icon").FirstOrDefault();

            button.gameObject.SetActive(true);
            return button.gameObject;
        }
    }
}
