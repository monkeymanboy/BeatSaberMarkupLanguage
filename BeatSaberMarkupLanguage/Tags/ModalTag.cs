using BeatSaberMarkupLanguage.Components;
using HMUI;
using IPA.Utilities;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ModalTag : BSMLTag
    {
        public override string[] Aliases => new[] { "modal" };

        public override GameObject CreateObject(Transform parent)
        {
            ModalView yoinkFromView = Resources.FindObjectsOfTypeAll<ModalView>().First(x => x.name == "DropdownTableView");
            ModalView modalView = GameObject.Instantiate(yoinkFromView, parent);
            modalView.SetField("_presentPanelAnimations", yoinkFromView.GetField<PanelAnimationSO, ModalView>("_presentPanelAnimations"));
            modalView.SetField("_dismissPanelAnimation", yoinkFromView.GetField<PanelAnimationSO, ModalView>("_dismissPanelAnimation"));
            modalView.SetField("_container", BeatSaberUI.DiContainer);
            modalView.GetComponent<VRGraphicRaycaster>().SetField("_physicsRaycaster", BeatSaberUI.PhysicsRaycasterWithCache);

            GameObject.Destroy(modalView.GetComponent<TableView>());
            GameObject.Destroy(modalView.GetComponent<TableViewScroller>());
            GameObject.Destroy(modalView.GetComponent<ScrollRect>());

            foreach (RectTransform child in modalView.transform)
            {
                if (child.name == "BG")
                {
                    child.anchoredPosition = Vector2.zero;
                    child.sizeDelta = Vector2.zero;
                }
                else
                {
                    GameObject.Destroy(child.gameObject);
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
