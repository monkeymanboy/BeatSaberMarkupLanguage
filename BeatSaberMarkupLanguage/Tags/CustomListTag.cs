using BeatSaberMarkupLanguage.Components;
using HMUI;
using IPA.Utilities;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;

namespace BeatSaberMarkupLanguage.Tags
{
    public class CustomListTag : BSMLTag
    {
        public override string[] Aliases => new[] { "custom-list" };
        public override bool AddChildren { get => false; }

        public override GameObject CreateObject(Transform parent)
        {
            RectTransform container = new GameObject("BSMLCustomListContainer", typeof(RectTransform)).transform as RectTransform;
            LayoutElement layoutElement = container.gameObject.AddComponent<LayoutElement>();
            container.SetParent(parent, false);

            GameObject gameObject = new GameObject();
            gameObject.transform.SetParent(container, false);
            gameObject.name = "BSMLCustomList";
            gameObject.SetActive(false);

            gameObject.AddComponent<ScrollRect>();
            gameObject.AddComponent(Resources.FindObjectsOfTypeAll<Canvas>().First(x => x.name == "DropdownTableView"));
            gameObject.AddComponent<VRGraphicRaycaster>().SetField("_physicsRaycaster", BeatSaberUI.PhysicsRaycasterWithCache);
            gameObject.AddComponent<Touchable>();
            gameObject.AddComponent<EventSystemListener>();

            TableView tableView = gameObject.AddComponent<BSMLTableView>();
            CustomCellListTableData tableData = container.gameObject.AddComponent<CustomCellListTableData>();
            tableData.tableView = tableView;

            tableView.SetField<TableView, TableView.CellsGroup[]>("_preallocatedCells", new TableView.CellsGroup[0]);
            tableView.SetField<TableView, bool>("_isInitialized", false);

            RectTransform viewport = new GameObject("Viewport").AddComponent<RectTransform>();
            viewport.SetParent(gameObject.GetComponent<RectTransform>(), false);
            gameObject.GetComponent<ScrollRect>().viewport = viewport;
            viewport.gameObject.AddComponent<RectMask2D>();

            (viewport.transform as RectTransform).anchorMin = new Vector2(0f, 0f);
            (viewport.transform as RectTransform).anchorMax = new Vector2(1f, 1f);
            (viewport.transform as RectTransform).sizeDelta = new Vector2(0f, 0f);
            (viewport.transform as RectTransform).anchoredPosition = new Vector3(0f, 0f);

            (tableView.transform as RectTransform).anchorMin = new Vector2(0f, 0f);
            (tableView.transform as RectTransform).anchorMax = new Vector2(1f, 1f);
            (tableView.transform as RectTransform).sizeDelta = new Vector2(0f, 0f);
            (tableView.transform as RectTransform).anchoredPosition = new Vector3(0f, 0f);

            tableView.SetDataSource(tableData, true);
            return container.gameObject;
        }
    }
}
