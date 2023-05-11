using BeatSaberMarkupLanguage.Components;
using HMUI;
using IPA.Utilities;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ListTag : BSMLTag
    {
        private Canvas canvasTemplate;

        public override string[] Aliases => new[] { "list" };

        public override GameObject CreateObject(Transform parent)
        {
            RectTransform container = new GameObject("BSMLListContainer", typeof(RectTransform)).transform as RectTransform;
            LayoutElement layoutElement = container.gameObject.AddComponent<LayoutElement>();
            container.SetParent(parent, false);

            GameObject gameObject = new GameObject();
            gameObject.transform.SetParent(container, false);
            gameObject.name = "BSMLList";
            gameObject.SetActive(false);

            if (canvasTemplate == null)
                canvasTemplate = Resources.FindObjectsOfTypeAll<Canvas>().First(x => x.name == "DropdownTableView");

            gameObject.AddComponent<ScrollRect>();
            gameObject.AddComponent(canvasTemplate);
            gameObject.AddComponent<VRGraphicRaycaster>().SetField("_physicsRaycaster", BeatSaberUI.PhysicsRaycasterWithCache);
            gameObject.AddComponent<Touchable>();
            gameObject.AddComponent<EventSystemListener>();
            ScrollView scrollView = gameObject.AddComponent<ScrollView>();

            TableView tableView = gameObject.AddComponent<BSMLTableView>();
            CustomListTableData tableData = container.gameObject.AddComponent<CustomListTableData>();
            tableData.tableView = tableView;
            
            tableView.SetField<TableView, TableView.CellsGroup[]>("_preallocatedCells", new TableView.CellsGroup[0]);
            tableView.SetField<TableView, bool>("_isInitialized", false);
            tableView.SetField("_scrollView", scrollView);

            RectTransform viewport = new GameObject("Viewport").AddComponent<RectTransform>();
            viewport.SetParent(gameObject.GetComponent<RectTransform>(), false);
            viewport.gameObject.AddComponent<RectMask2D>();
            gameObject.GetComponent<ScrollRect>().viewport = viewport;

            RectTransform content = new GameObject("Content").AddComponent<RectTransform>();
            content.SetParent(viewport, false);

            scrollView.SetField("_contentRectTransform", content);
            scrollView.SetField("_viewport", viewport);
            scrollView.SetField("_platformHelper", BeatSaberUI.PlatformHelper);

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
