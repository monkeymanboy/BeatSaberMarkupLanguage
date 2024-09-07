﻿using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Parser;
using BGLib.Polyglot;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class TabSelector : MonoBehaviour
    {
        public TextSegmentedControl TextSegmentedControl;
        public BSMLParserParams ParserParams;
        public string TabTag;
        public string LeftButtonTag;
        public string RightButtonTag;
        private readonly List<Tab> tabs = new();

        private int pageCount = -1;
        private int currentPage = 0;

        private Button leftButton;
        private Button rightButton;

        private bool shouldRefresh;

        public int PageCount
        {
            get => pageCount;
            set
            {
                pageCount = value;
                if (tabs.Count > 0)
                {
                    Refresh();
                }
            }
        }

        public void Setup()
        {
            tabs.Clear();
            foreach (GameObject gameObject in ParserParams.GetObjectsWithTag(TabTag))
            {
                Tab tab = gameObject.GetComponent<Tab>();
                tabs.Add(tab);
                tab.Selector = this;
            }

            if (LeftButtonTag != null)
            {
                leftButton = ParserParams.GetObjectsWithTag(LeftButtonTag).FirstOrDefault().GetComponent<Button>();
            }

            if (leftButton != null)
            {
                leftButton.onClick.AddListener(PageLeft);
            }

            if (RightButtonTag != null)
            {
                rightButton = ParserParams.GetObjectsWithTag(RightButtonTag).FirstOrDefault().GetComponent<Button>();
            }

            if (rightButton != null)
            {
                rightButton.onClick.AddListener(PageRight);
            }

            Refresh();
            TextSegmentedControl.didSelectCellEvent -= TabSelected;
            TextSegmentedControl.didSelectCellEvent += TabSelected;
            TextSegmentedControl.SelectCellWithNumber(0);
            TabSelected(TextSegmentedControl, 0);
        }

        public void Refresh()
        {
            if (!isActiveAndEnabled)
            {
                shouldRefresh = true;
                return;
            }

            shouldRefresh = false;
            List<Tab> visibleTabs = tabs.Where(x => x.IsVisible).ToList();
            if (PageCount == -1)
            {
                SetSegmentedControlTexts(visibleTabs);
            }
            else
            {
                if (currentPage < 0)
                {
                    currentPage = 0;
                }

                if (currentPage > (visibleTabs.Count - 1) / pageCount)
                {
                    currentPage = (visibleTabs.Count - 1) / pageCount;
                }

                SetSegmentedControlTexts(visibleTabs.Skip(PageCount * currentPage).Take(PageCount).ToList());
                if (leftButton != null)
                {
                    leftButton.interactable = currentPage > 0;
                }

                if (rightButton != null)
                {
                    rightButton.interactable = currentPage < (visibleTabs.Count - 1) / pageCount;
                }

                TabSelected(null, 0);
            }
        }

        protected void OnEnable()
        {
            if (shouldRefresh)
            {
                Refresh();
            }
        }

        private void TabSelected(SegmentedControl segmentedControl, int index)
        {
            if (PageCount != -1)
            {
                index += PageCount * currentPage;
            }

            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].gameObject.SetActive(false);
            }

            if (index >= tabs.Where(x => x.IsVisible).Count())
            {
                return;
            }

            tabs.Where(x => x.IsVisible).ElementAt(index).gameObject.SetActive(true);
        }

        private void SetSegmentedControlTexts(List<Tab> tabs)
        {
            string[] texts = new string[tabs.Count];

            for (int i = 0; i < tabs.Count; i++)
            {
                Tab tab = tabs[i];

                if (!string.IsNullOrEmpty(tab.TabKey))
                {
                    texts[i] = Localization.Get(tab.TabKey);
                }
                else
                {
                    texts[i] = tab.TabName;
                }
            }

            TextSegmentedControl.SetTexts(texts);
        }

        private void PageLeft()
        {
            currentPage--;
            Refresh();
        }

        private void PageRight()
        {
            currentPage++;
            Refresh();
        }
    }
}
