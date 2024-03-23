using BeatSaberMarkupLanguage.Components;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;

namespace BeatSaberMarkupLanguage.Tags
{
    public class CustomListTag : PrefabBSMLTag
    {
        private Canvas canvasTemplate;

        public override string[] Aliases => new[] { "custom-list" };

        public override bool AddChildren { get => false; }

        protected override PrefabParams CreatePrefab()
        {
            GameObject containerObject = new("BSMLCustomListContainer", typeof(RectTransform), typeof(LayoutElement))
            {
                layer = 5,
            };

            GameObject gameObject = new("BSMLCustomList")
            {
                layer = 5,
            };

            gameObject.transform.SetParent(containerObject.transform, false);
            gameObject.SetActive(false);

            if (canvasTemplate == null)
            {
                canvasTemplate = BeatSaberUI.DiContainer.Resolve<GameplaySetupViewController>()._playerSettingsPanelController._noteJumpStartBeatOffsetDropdown._simpleTextDropdown._tableView.GetComponent<Canvas>();
            }

            gameObject.AddComponent<ScrollRect>();
            gameObject.AddComponent(canvasTemplate);
            BeatSaberUI.DiContainer.InstantiateComponent<VRGraphicRaycaster>(gameObject);
            gameObject.AddComponent<Touchable>();
            gameObject.AddComponent<EventSystemListener>();
            ScrollView scrollView = BeatSaberUI.DiContainer.InstantiateComponent<ScrollView>(gameObject);

            TableView tableView = gameObject.AddComponent<BSMLTableView>();
            CustomCellListTableData tableData = containerObject.AddComponent<CustomCellListTableData>();
            tableData.tableView = tableView;

            tableView._preallocatedCells = new TableView.CellsGroup[0];
            tableView._isInitialized = false;
            tableView._scrollView = scrollView;

            RectTransform viewport = new GameObject("Viewport").AddComponent<RectTransform>();
            viewport.SetParent(gameObject.GetComponent<RectTransform>(), false);
            gameObject.GetComponent<ScrollRect>().viewport = viewport;
            viewport.gameObject.AddComponent<RectMask2D>();

            RectTransform content = new GameObject("Content").AddComponent<RectTransform>();
            content.SetParent(viewport, false);

            scrollView._contentRectTransform = content;
            scrollView._viewport = viewport;

            (viewport.transform as RectTransform).anchorMin = new Vector2(0f, 0f);
            (viewport.transform as RectTransform).anchorMax = new Vector2(1f, 1f);
            (viewport.transform as RectTransform).sizeDelta = new Vector2(0f, 0f);
            (viewport.transform as RectTransform).anchoredPosition = new Vector3(0f, 0f);

            (tableView.transform as RectTransform).anchorMin = new Vector2(0f, 0f);
            (tableView.transform as RectTransform).anchorMax = new Vector2(1f, 1f);
            (tableView.transform as RectTransform).sizeDelta = new Vector2(0f, 0f);
            (tableView.transform as RectTransform).anchoredPosition = new Vector3(0f, 0f);

            return new PrefabParams(containerObject);
        }
    }
}
