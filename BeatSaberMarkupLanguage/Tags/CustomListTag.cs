using System.Linq;
using BeatSaberMarkupLanguage.Components;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;

namespace BeatSaberMarkupLanguage.Tags
{
    public class CustomListTag : BSMLTag
    {
        private Canvas canvasTemplate;

        public override string[] Aliases => new[] { "custom-list" };

        public override bool AddChildren { get => false; }

        public override GameObject CreateObject(Transform parent)
        {
            RectTransform container = (RectTransform)new GameObject("BSMLCustomListContainer", typeof(RectTransform)).transform;
            LayoutElement layoutElement = container.gameObject.AddComponent<LayoutElement>();
            container.SetParent(parent, false);

            GameObject gameObject = new("BSMLCustomList");
            gameObject.transform.SetParent(container, false);
            gameObject.SetActive(false);

            if (canvasTemplate == null)
            {
                canvasTemplate = Resources.FindObjectsOfTypeAll<Canvas>().First(x => x.name == "DropdownTableView");
            }

            gameObject.AddComponent<ScrollRect>();
            gameObject.AddComponent(canvasTemplate);
            DiContainer.InstantiateComponent<VRGraphicRaycaster>(gameObject);
            gameObject.AddComponent<Touchable>();
            gameObject.AddComponent<EventSystemListener>();
            ScrollView scrollView = DiContainer.InstantiateComponent<ScrollView>(gameObject);

            TableView tableView = gameObject.AddComponent<BSMLTableView>();
            CustomCellListTableData tableData = container.gameObject.AddComponent<CustomCellListTableData>();
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

            // Changed the bool param to "false", as it would otherwise trigger LazyInit earlier than we want it to
            tableView.SetDataSource(tableData, false);
            return container.gameObject;
        }
    }
}
