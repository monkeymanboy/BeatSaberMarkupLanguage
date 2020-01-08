using BeatSaberMarkupLanguage.Components;
using HMUI;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BeatSaberMarkupLanguage.Components.Strokable;
using Image = UnityEngine.UI.Image;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ButtonWithIconTag : BSMLTag
    {
        public override string[] Aliases => new[] { "button-with-icon", "icon-button" };

        public override GameObject CreateObject(Transform parent)
        {
            Button button = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<Button>().Last(x => (x.name == "PracticeButton" && x.transform.parent.name == "PlayButtons")), parent, false);
            button.name = "BSMLIconButton";
            button.interactable = true;

            Object.Destroy(button.GetComponent<HoverHint>());
            Object.Destroy(button.GetComponent<LocalizedHoverHint>());
            button.gameObject.AddComponent<ExternalComponents>().components.Add(button.GetComponentsInChildren<HorizontalLayoutGroup>().First(x => x.name == "Content"));

            Image glowImage = button.gameObject.GetComponentsInChildren<Image>(true).Where(x => x.gameObject.name == "Glow").FirstOrDefault();
            if (glowImage != null)
            {
                Glowable glowable = button.gameObject.AddComponent<Glowable>();
                glowable.image = glowImage;
                glowable.SetGlow("none");
            }

            Image strokeImage = button.gameObject.GetComponentsInChildren<Image>(true).Where(x => x.gameObject.name == "Stroke").FirstOrDefault();
            if (strokeImage != null)
            {
                Strokable strokable = button.gameObject.AddComponent<Strokable>();
                strokable.image = strokeImage;
                strokable.SetType(StrokeType.Regular);
            }

            Image iconImage = button.gameObject.GetComponentsInChildren<Image>(true).Where(x => x.gameObject.name == "Icon").FirstOrDefault();
            if (iconImage != null)
            {
                ButtonIconImage btnIcon = button.gameObject.AddComponent<ButtonIconImage>();
                btnIcon.image = iconImage;
            }

            return button.gameObject; 
        }
    }
}
