using BeatSaberMarkupLanguage.Parser;
using HMUI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    public class TabSelector : MonoBehaviour
    {
        public TextSegmentedControl textSegmentedControl;
        public BSMLParserParams parserParams;
        public string tabTag;
        private List<Tab> tabs = new List<Tab>();
        public void Setup()
        {
            tabs.Clear();
            foreach(GameObject gameObject in parserParams.GetObjectsWithTag(tabTag))
            {
                tabs.Add(gameObject.GetComponent<Tab>());
            }
            textSegmentedControl.SetTexts(tabs.Select(x => x.tabName).ToArray());
            textSegmentedControl.didSelectCellEvent -= TabSelected;
            textSegmentedControl.didSelectCellEvent += TabSelected;
            textSegmentedControl.SelectCellWithNumber(0);
            TabSelected(textSegmentedControl, 0);
        }
        private void TabSelected(SegmentedControl segmentedControl, int index)
        {
            for(int i = 0; i < tabs.Count; i++)
            {
                tabs[i].gameObject.SetActive(i == index);
            }
        }
    }
}
