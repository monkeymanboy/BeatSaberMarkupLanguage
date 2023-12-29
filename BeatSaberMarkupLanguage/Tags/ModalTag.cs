using System.Linq;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ModalTag : BSMLTag
    {
        private ModalView modalViewTemplate;

        public override string[] Aliases => new[] { "modal" };

        public override GameObject CreateObject(Transform parent)
        {
            if (modalViewTemplate == null)
            {
                modalViewTemplate = Resources.FindObjectsOfTypeAll<ModalView>().Where(mv => mv.name == "DropdownTableView" && mv.transform.parent != null && mv.transform.parent.name == "ColorSchemeDropDown").First();
            }

            ModalView modalView = DiContainer.InstantiatePrefabForComponent<ModalView>(modalViewTemplate, parent);
            modalView.name = "BSMLModal";
            modalView._presentPanelAnimations = modalViewTemplate._presentPanelAnimations;
            modalView._dismissPanelAnimation = modalViewTemplate._dismissPanelAnimation;

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
