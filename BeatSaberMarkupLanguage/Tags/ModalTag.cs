using BeatSaberMarkupLanguage.Components;
using BS_Utils.Utilities;
using HMUI;
using System.Linq;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ModalTag : BSMLTag
    {
        public override string[] Aliases => new[] { "modal" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = new GameObject();
            gameObject.SetActive(false);
            gameObject.name = "BSMLModalView";

            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.SetParent(parent, false);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(0, 0);

            ModalView modalView = gameObject.AddComponent<ModalView>();
            ModalView yoinkFromView = Resources.FindObjectsOfTypeAll<ModalView>().First(x => x.name == "TableView");
            modalView.SetPrivateField("_presentPanelAnimations", yoinkFromView.GetPrivateField<PanelAnimationSO>("_presentPanelAnimations"));
            modalView.SetPrivateField("_dismissPanelAnimation", yoinkFromView.GetPrivateField<PanelAnimationSO>("_dismissPanelAnimation"));

            GameObject child = new GameObject();
            child.transform.SetParent(rectTransform, false);
            child.name = "Shadow";
            RectTransform shadowTransform = child.gameObject.AddComponent<RectTransform>();
            shadowTransform.anchorMin = new Vector2(0, 0);
            shadowTransform.anchorMax = new Vector2(1, 1);
            shadowTransform.sizeDelta = new Vector2(10, 10);
            child.gameObject.AddComponent<Backgroundable>().ApplyBackground("round-rect-panel-shadow");

            child = new GameObject();
            child.transform.SetParent(rectTransform, false);
            child.name = "Content";
            RectTransform backgroundTransform = child.gameObject.AddComponent<RectTransform>();
            backgroundTransform.anchorMin = new Vector2(0, 0);
            backgroundTransform.anchorMax = new Vector2(1, 1);
            backgroundTransform.sizeDelta = new Vector2(0, 0);

            Backgroundable backgroundable = child.gameObject.AddComponent<Backgroundable>();
            backgroundable.ApplyBackground("round-rect-panel");
            backgroundable.background.color = new Color(0.706f, 0.706f, 0.706f, 1);
            backgroundable.background.material = Resources.FindObjectsOfTypeAll<UnityEngine.UI.Image>().First(x => x.material?.name=="UIFogBG").material;

            ExternalComponents externalComponents = child.AddComponent<ExternalComponents>();
            externalComponents.components.Add(modalView);
            externalComponents.components.Add(rectTransform);

            return child;
        }
    }
}
