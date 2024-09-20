using System.Collections.Generic;
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
        private readonly List<Tab> tabs = new();

        [SerializeField]
        private TextSegmentedControl textSegmentedControl;

        [SerializeField]
        private string tabTag;

        [SerializeField]
        private string leftButtonTag;

        [SerializeField]
        private string rightButtonTag;

        private int pageCount = -1;
        private int currentPage = 0;

        private Button leftButton;
        private Button rightButton;

        private bool shouldRefresh;

        public BSMLParserParams ParserParams { get; internal set; }

        public TextSegmentedControl TextSegmentedControl
        {
            get => textSegmentedControl;
            set => textSegmentedControl = value;
        }

        public string TabTag
        {
            get => tabTag;
            set => tabTag = value;
        }

        public string LeftButtonTag
        {
            get => leftButtonTag;
            set => leftButtonTag = value;
        }

        public string RightButtonTag
        {
            get => rightButtonTag;
            set => rightButtonTag = value;
        }

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
            foreach (GameObject gameObject in ParserParams.GetObjectsWithTag(tabTag))
            {
                Tab tab = gameObject.GetComponent<Tab>();
                tabs.Add(tab);
                tab.Selector = this;
            }

            if (leftButtonTag != null)
            {
                leftButton = ParserParams.GetObjectsWithTag(leftButtonTag).FirstOrDefault().GetComponent<Button>();
            }

            if (leftButton != null)
            {
                leftButton.onClick.AddListener(PageLeft);
            }

            if (rightButtonTag != null)
            {
                rightButton = ParserParams.GetObjectsWithTag(rightButtonTag).FirstOrDefault().GetComponent<Button>();
            }

            if (rightButton != null)
            {
                rightButton.onClick.AddListener(PageRight);
            }

            Refresh();
            textSegmentedControl.didSelectCellEvent -= TabSelected;
            textSegmentedControl.didSelectCellEvent += TabSelected;
            textSegmentedControl.SelectCellWithNumber(0);
            TabSelected(textSegmentedControl, 0);
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
    }
}
