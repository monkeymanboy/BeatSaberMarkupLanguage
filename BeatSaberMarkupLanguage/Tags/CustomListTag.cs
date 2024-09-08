﻿using BeatSaberMarkupLanguage.Components;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;

namespace BeatSaberMarkupLanguage.Tags
{
    public class CustomListTag : ListTag
    {
        private Canvas canvasTemplate;

        public override string[] Aliases => new[] { "custom-list" };

        public override bool AddChildren { get => false; }

        public override GameObject CreateObject(Transform parent)
        {
            GameObject containerObject = new("BSMLCustomListContainer", typeof(LayoutElement))
            {
                layer = 5,
            };

            RectTransform container = (RectTransform)containerObject.transform;
            container.SetParent(parent, false);
            container.anchorMin = Vector2.zero;
            container.anchorMax = Vector2.one;
            container.sizeDelta = Vector2.zero;
            container.anchoredPosition = Vector2.zero;

            GameObject gameObject = new("BSMLCustomList")
            {
                layer = 5,
            };
            gameObject.SetActive(false);

            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.SetParent(container, false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;

            if (canvasTemplate == null)
            {
                canvasTemplate = DiContainer.Resolve<GameplaySetupViewController>()._playerSettingsPanelController._noteJumpStartBeatOffsetDropdown._simpleTextDropdown._tableView.GetComponent<Canvas>();
            }

            gameObject.AddComponent<ScrollRect>();
            gameObject.AddComponent(canvasTemplate);
            DiContainer.InstantiateComponent<VRGraphicRaycaster>(gameObject);
            gameObject.AddComponent<Touchable>();
            gameObject.AddComponent<EventSystemListener>();
            ScrollView scrollView = DiContainer.InstantiateComponent<BSMLScrollView>(gameObject);

            TableView tableView = gameObject.AddComponent<BSMLTableView>();
            CustomCellListTableData tableData = containerObject.AddComponent<CustomCellListTableData>();
            tableData.TableView = tableView;

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

            viewport.anchorMin = Vector2.zero;
            viewport.anchorMax = Vector2.one;
            viewport.sizeDelta = Vector2.zero;
            viewport.anchoredPosition = Vector2.zero;

            // Changed the bool param to "false", as it would otherwise trigger LazyInit earlier than we want it to
            tableView.SetDataSource(tableData, false);
            return containerObject;
        }
    }
}
