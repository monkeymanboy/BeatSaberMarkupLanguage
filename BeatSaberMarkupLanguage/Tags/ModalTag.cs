using HMUI;
using IPA.Utilities;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ModalTag : BSMLTag
    {
        private ModalView modalViewTemplate;

        public override string[] Aliases => new[] { "modal" };

        public override GameObject CreateObject(Transform parent)
        {
            if (modalViewTemplate == null)
                modalViewTemplate = Resources.FindObjectsOfTypeAll<ModalView>().First(x => x.name == "DropdownTableView");
            ModalView modalView = Object.Instantiate(modalViewTemplate, parent);
            modalView.SetField("_presentPanelAnimations", modalViewTemplate.GetField<PanelAnimationSO, ModalView>("_presentPanelAnimations"));
            modalView.SetField("_dismissPanelAnimation", modalViewTemplate.GetField<PanelAnimationSO, ModalView>("_dismissPanelAnimation"));
            modalView.SetField("_container", BeatSaberUI.DiContainer);
            modalView.GetComponent<VRGraphicRaycaster>().SetField("_physicsRaycaster", BeatSaberUI.PhysicsRaycasterWithCache);

            Object.DestroyImmediate(modalView.GetComponent<TableView>());
            Object.DestroyImmediate(modalView.GetComponent<ScrollRect>());
            Object.DestroyImmediate(modalView.GetComponent<ScrollView>());
            Object.DestroyImmediate(modalView.GetComponent<EventSystemListener>());
            //GameObject.DestroyImmediate(modalView.GetComponent<Touchable>());
            //modalView.gameObject.AddComponent<CurvedCanvasSettings>();
            //modalView.gameObject.AddComponent<EventSystemListener>();

            foreach (RectTransform child in modalView.transform)
            {
                if (child.name == "BG")
                {
                    child.anchoredPosition = Vector2.zero;
                    child.sizeDelta = Vector2.zero;
                }
                else
                {
                    Object.Destroy(child.gameObject);
                }
            }

            RectTransform rectTransform = modalView.transform as RectTransform;
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(0, 0);

            return modalView.gameObject;
        }
    }
}
