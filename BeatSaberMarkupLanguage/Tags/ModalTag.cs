using BeatSaberMarkupLanguage.Util;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ModalTag : BSMLTag
    {
        private ModalView _modalViewTemplate;

        public override string[] Aliases => new[] { "modal" };

        public override void Setup()
        {
            base.Setup();
            _modalViewTemplate = DiContainer.Resolve<GameplaySetupViewController>().GetComponentOnChild<ModalView>("ColorsOverrideSettings/Settings/Detail/ColorSchemeDropDown/DropdownTableView");
        }

        public override GameObject CreateObject(Transform parent)
        {
            ModalView modalView = DiContainer.InstantiatePrefabForComponent<ModalView>(_modalViewTemplate, parent);
            modalView._presentPanelAnimations = _modalViewTemplate._presentPanelAnimations;
            modalView._dismissPanelAnimation = _modalViewTemplate._dismissPanelAnimation;

            Object.DestroyImmediate(modalView.GetComponent<TableView>());
            Object.DestroyImmediate(modalView.GetComponent<ScrollRect>());
            Object.DestroyImmediate(modalView.GetComponent<ScrollView>());
            Object.DestroyImmediate(modalView.GetComponent<EventSystemListener>());

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

            modalView.gameObject.SetActive(false);

            return modalView.gameObject;
        }
    }
}
