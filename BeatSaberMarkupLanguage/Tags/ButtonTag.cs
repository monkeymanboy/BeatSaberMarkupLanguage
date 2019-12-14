using BeatSaberMarkupLanguage.Components;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ButtonTag : BSMLTag
    {
        public override string[] Aliases => new[] { "button" };

        public override GameObject CreateObject(Transform parent)
        {
            Button button = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<Button>().Last(x => (x.name == (parent.GetComponent<StartMiddleEndButtonsGroup>() == null ? "PlayButton" : "CreditsButton"))), parent, false);
            button.name = "BSMLButton";
            button.interactable = true;
            Polyglot.LocalizedTextMeshProUGUI localizer = button.GetComponentInChildren<Polyglot.LocalizedTextMeshProUGUI>();
            if (localizer != null)
                GameObject.Destroy(localizer);
            button.gameObject.AddComponent<ExternalComponents>().components.Add(button.GetComponentInChildren<TextMeshProUGUI>());

            Image glowImage = button.gameObject.GetComponentsInChildren<Image>(true).Where(x => x.gameObject.name == "Glow").FirstOrDefault();
            if(glowImage != null)
            {
                Glowable glowable = button.gameObject.AddComponent<Glowable>();
                glowable.image = glowImage;
                glowable.SetGlow("none");
            }

            return button.gameObject;
        }
    }
}
