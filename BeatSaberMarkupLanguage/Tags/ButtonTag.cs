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
        public virtual string PrefabButton => "PracticeButton";

        public override GameObject CreateObject(Transform parent)
        {
            Button button = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<Button>().Last(x => (x.name == PrefabButton)), parent, false);
            button.name = "BSMLButton";
            button.interactable = true;
            Polyglot.LocalizedTextMeshProUGUI localizer = button.GetComponentInChildren<Polyglot.LocalizedTextMeshProUGUI>();
            if (localizer != null)
                GameObject.Destroy(localizer);
            ExternalComponents externalComponents = button.gameObject.AddComponent<ExternalComponents>();
            externalComponents.components.Add(button.GetComponentInChildren<TextMeshProUGUI>());

            HorizontalLayoutGroup horiztonalLayoutGroup = button.GetComponentInChildren<HorizontalLayoutGroup>();
            if (horiztonalLayoutGroup != null)
                externalComponents.components.Add(horiztonalLayoutGroup);

            /*
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
            }*/

            return button.gameObject;
        }
    }
}
