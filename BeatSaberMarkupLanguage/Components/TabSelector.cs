using BeatSaberMarkupLanguage.Parser;
using HMUI;
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
        public int PageCount {
            get => pageCount;
            set
            {
                pageCount = value;
                if(tabs.Count > 0) Refresh();
            }
        }

        private int currentPage = 0;

        private Button leftButton;
        private Button rightButton;

        private int lastClickedPage;
        private int lastClickedIndex;

        public void Setup()
        {
            tabs.Clear();
            foreach(GameObject gameObject in parserParams.GetObjectsWithTag(tabTag))
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
            lastClickedPage = currentPage;
            lastClickedIndex = index;
            if (PageCount != -1) index += PageCount * currentPage;
            for(int i = 0; i < tabs.Count; i++)
            {
                tabs[i].gameObject.SetActive(i == index);
            }
        }
        public void Refresh()
        {
            if (PageCount == -1)
            {
                textSegmentedControl.SetTexts(tabs.Select(x => x.TabName).ToArray());
            }
            else
            {
                if (currentPage < 0)
                    currentPage = 0;
                if(currentPage > (tabs.Count - 1) / pageCount)
                    currentPage = (tabs.Count - 1) / pageCount;
                textSegmentedControl.SetTexts(tabs.Select(x => x.TabName).Skip(PageCount * currentPage).Take(PageCount).ToArray());
                if(leftButton != null)
                    leftButton.interactable = currentPage > 0;
                if(rightButton != null)
                    rightButton.interactable = currentPage < (tabs.Count - 1) / pageCount;
                textSegmentedControl.SelectCellWithNumber(lastClickedPage == currentPage? lastClickedIndex : -1);
            }
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
