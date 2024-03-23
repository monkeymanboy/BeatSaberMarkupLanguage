using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ModalTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "modal" };

        protected override PrefabParams CreatePrefab()
        {
            ModalView modalViewTemplate = BeatSaberUI.DiContainer.Resolve<GameplaySetupViewController>()._colorsOverrideSettingsPanelController._colorSchemeDropDown._modalView;
            ModalView modalView = BeatSaberUI.DiContainer.InstantiatePrefabForComponent<ModalView>(modalViewTemplate);
            modalView.name = "BSMLModal";
            modalView.gameObject.SetActive(false);

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

            RectTransform rectTransform = (RectTransform)modalView.transform;
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(0, 0);

            return new PrefabParams(modalView.gameObject, false);
        }
    }
}
