using BeatSaberMarkupLanguage.Components;
using HMUI;
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
            TextMeshProUGUI textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.richText = true;
            externalComponents.components.Add(textMesh);

            GameObject.Destroy(button.transform.Find("Content").GetComponent<LayoutElement>());

            ContentSizeFitter buttonSizeFitter = button.gameObject.AddComponent<ContentSizeFitter>();
            buttonSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            buttonSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            LayoutGroup stackLayoutGroup = button.GetComponentInChildren<LayoutGroup>();
            if (stackLayoutGroup != null)
                externalComponents.components.Add(stackLayoutGroup);

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
