using System.Linq;
using BeatSaberMarkupLanguage.Components;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ButtonWithIconTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "button-with-icon", "icon-button" };

        protected override PrefabParams CreatePrefab()
        {
            Button button = Object.Instantiate(BeatSaberUI.DiContainer.Resolve<StandardLevelDetailViewController>()._standardLevelDetailView.practiceButton);
            button.name = "BSMLIconButton";
            button.interactable = true;

            GameObject gameObject = button.gameObject;

            Object.Destroy(button.GetComponent<HoverHint>());
            Object.Destroy(button.GetComponent<LocalizedHoverHint>());
            gameObject.AddComponent<ExternalComponents>().components.Add(button.GetComponentsInChildren<LayoutGroup>().Where(x => x.name == "Content").First());

            Transform contentTransform = button.transform.Find("Content");
            Object.Destroy(contentTransform.Find("Text").gameObject);
            Image iconImage = new GameObject("Icon")
            {
                layer = 5,
            }.AddComponent<ImageView>();
            iconImage.material = Utilities.ImageResources.NoGlowMat;
            iconImage.rectTransform.SetParent(contentTransform, false);
            iconImage.rectTransform.sizeDelta = new Vector2(20f, 20f);
            iconImage.sprite = Utilities.ImageResources.BlankSprite;
            iconImage.preserveAspect = true;
            if (iconImage != null)
            {
                ButtonIconImage btnIcon = gameObject.AddComponent<ButtonIconImage>();
                btnIcon.image = iconImage;
            }

            Object.Destroy(button.transform.Find("Content").GetComponent<LayoutElement>());

            ContentSizeFitter buttonSizeFitter = gameObject.AddComponent<ContentSizeFitter>();
            buttonSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            buttonSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            return new PrefabParams(gameObject);
        }
    }
}
