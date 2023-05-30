using BeatSaberMarkupLanguage.Parser;
using HMUI;
using Polyglot;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Components
{
    public class TabSelector : MonoBehaviour
    {
        public TextSegmentedControl textSegmentedControl;
        public BSMLParserParams parserParams;
        public string tabTag;
        public string leftButtonTag;
        public string rightButtonTag;
        private List<Tab> tabs = new List<Tab>();

        private int pageCount = -1;
        public int PageCount
        {
            get => pageCount;
            set
            {
                pageCount = value;
                if (tabs.Count > 0) Refresh();
            }
        }

        private int currentPage = 0;

        private Button leftButton;
        private Button rightButton;

        private bool shouldRefresh;

        public void Setup()
        {
            tabs.Clear();
            foreach (GameObject gameObject in parserParams.GetObjectsWithTag(tabTag))
            {
                Tab tab = gameObject.GetComponent<Tab>();
                tabs.Add(tab);
                tab.selector = this;
            }
            if (leftButtonTag != null) leftButton = parserParams.GetObjectsWithTag(leftButtonTag).FirstOrDefault().GetComponent<Button>();
            if (leftButton != null) leftButton.onClick.AddListener(PageLeft);
            if (rightButtonTag != null) rightButton = parserParams.GetObjectsWithTag(rightButtonTag).FirstOrDefault().GetComponent<Button>();
            if (rightButton != null) rightButton.onClick.AddListener(PageRight);
            Refresh();
            textSegmentedControl.didSelectCellEvent -= TabSelected;
            textSegmentedControl.didSelectCellEvent += TabSelected;
            textSegmentedControl.SelectCellWithNumber(0);
            TabSelected(textSegmentedControl, 0);
        }
        private void TabSelected(SegmentedControl segmentedControl, int index)
        {
            if (PageCount != -1) index += PageCount * currentPage;
            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].gameObject.SetActive(false);
            }
            if (index >= tabs.Where(x => x.IsVisible).Count())
                return;
            tabs.Where(x => x.IsVisible).ElementAt(index).gameObject.SetActive(true);
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
                    currentPage = 0;
                if (currentPage > (visibleTabs.Count - 1) / pageCount)
                    currentPage = (visibleTabs.Count - 1) / pageCount;
                SetSegmentedControlTexts(visibleTabs.Skip(PageCount * currentPage).Take(PageCount).ToList());
                if (leftButton != null)
                    leftButton.interactable = currentPage > 0;
                if (rightButton != null)
                    rightButton.interactable = currentPage < (visibleTabs.Count - 1) / pageCount;

                TabSelected(null, 0);
            }
        }
        private void SetSegmentedControlTexts(List<Tab> tabs)
        {
            var texts = new string[tabs.Count];

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

            textSegmentedControl.SetTexts(texts);
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

        private void OnEnable()
        {
            if (shouldRefresh)
                Refresh();
        }
    }
}
