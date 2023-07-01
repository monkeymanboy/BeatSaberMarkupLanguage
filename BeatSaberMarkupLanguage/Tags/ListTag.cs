using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Util;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ListTag : BSMLTag
    {
        private Canvas _canvasTemplate;

        public override string[] Aliases => new[] { "list" };

        public override void Setup()
        {
            base.Setup();
            _canvasTemplate = DiContainer.Resolve<GameplaySetupViewController>().GetComponentOnChild<Canvas>("ColorsOverrideSettings/Settings/Detail/ColorSchemeDropDown/DropdownTableView");
        }

        public override GameObject CreateObject(Transform parent)
        {
            RectTransform container = new GameObject("BSMLListContainer", typeof(RectTransform)).transform as RectTransform;
            container.gameObject.AddComponent<LayoutElement>();
            container.SetParent(parent, false);

            GameObject gameObject = new("BSMLList")
            {
                layer = 5,
            };

            gameObject.transform.SetParent(container, false);
            gameObject.SetActive(false);

            gameObject.AddComponent<ScrollRect>();
            gameObject.AddComponent(_canvasTemplate);
            DiContainer.InstantiateComponent<VRGraphicRaycaster>(gameObject);
            gameObject.AddComponent<Touchable>();
            gameObject.AddComponent<EventSystemListener>();
            ScrollView scrollView = DiContainer.InstantiateComponent<ScrollView>(gameObject);

            TableView tableView = gameObject.AddComponent<BSMLTableView>();
            CustomListTableData tableData = container.gameObject.AddComponent<CustomListTableData>();
            tableData.tableView = tableView;

            tableView._preallocatedCells = new TableView.CellsGroup[0];
            tableView._isInitialized = false;
            tableView._scrollView = scrollView;

            RectTransform viewport = new GameObject("Viewport").AddComponent<RectTransform>();
            viewport.SetParent(gameObject.GetComponent<RectTransform>(), false);
            viewport.gameObject.AddComponent<RectMask2D>();
            gameObject.GetComponent<ScrollRect>().viewport = viewport;

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

            tableView.SetDataSource(tableData, false);
            return container.gameObject;
        }
    }
}
