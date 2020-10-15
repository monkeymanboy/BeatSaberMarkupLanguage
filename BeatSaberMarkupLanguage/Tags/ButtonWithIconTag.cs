using BeatSaberMarkupLanguage.Components;
using HMUI;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static BeatSaberMarkupLanguage.Components.Strokable;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ButtonWithIconTag : BSMLTag
    {
        public override string[] Aliases => new[] { "button-with-icon", "icon-button" };

        public override GameObject CreateObject(Transform parent)
        {
            Button button = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<Button>().Last(x => x.name == "PracticeButton"), parent, false);
            button.name = "BSMLIconButton";
            button.interactable = true;

            Object.Destroy(button.GetComponent<HoverHint>());
            GameObject.Destroy(button.GetComponent<LocalizedHoverHint>());
            button.gameObject.AddComponent<ExternalComponents>().components.Add(button.GetComponentsInChildren<StackLayoutGroup>().First(x => x.name == "Content"));

            Transform contentTransform = button.transform.Find("Content");
            GameObject.Destroy(contentTransform.Find("Text").gameObject);
            Image iconImage = new GameObject("Icon").AddComponent<ImageView>();
            iconImage.material = Utilities.ImageResources.NoGlowMat;
            iconImage.rectTransform.SetParent(contentTransform, false);
            iconImage.rectTransform.sizeDelta = new Vector2(20f, 20f);
            iconImage.sprite = Utilities.ImageResources.BlankSprite;
            if (iconImage != null)
            {
                ButtonIconImage btnIcon = button.gameObject.AddComponent<ButtonIconImage>();
                btnIcon.image = iconImage;
            }

            return button.gameObject; 
        }
    }
}
