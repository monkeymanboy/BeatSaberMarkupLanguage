using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using HMUI;
using System.Collections.Generic;
using TMPro;

namespace BeatSaberMarkupLanguage.ViewControllers
{
    public class TestViewController : BSMLResourceViewController
    {
        public override string ResourceName => "BeatSaberMarkupLanguage.Views.test.bsml";

        [UIValue("header")]
        public string headerText = "Header comes from code!";

        [UIComponent("sometext")]
        public TextMeshProUGUI text;

        [UIComponent("list")]
        public CustomCellListTableData tableData;

        [UIValue("contents")]
        public List<object> contents
        {
            get
            {
                List<object> list = new List<object>();
                list.Add(new TestListObject("first", false));
                list.Add(new TestListObject("second", true));
                list.Add(new TestListObject("third", true));
                list.Add(new TestListObject("fourth", false));
                list.Add(new TestListObject("fifth", true));
                list.Add(new TestListObject("sixth", false));
                return list;
            }
        }

        [UIAction("click")]
        private void ButtonPress()
        {
            text.text = "It works!";
        }

        [UIAction("cell click")]
        private void CellClick(TableView tableView, TestListObject testObj)
        {
            Logger.log.Debug("Clicked - " + testObj.title);
        }
    }
    public class TestListObject
    {
        [UIValue("title")]
        public string title;
        [UIValue("should-glow")]
        public bool shouldGlow;
        public TestListObject(string title, bool shouldGlow)
        {
            this.title = title;
            this.shouldGlow = shouldGlow;
        }

        [UIAction("button-click")]
        void ClickedButton()
        {
            Logger.log.Debug("Button - " + title);
        }
    }
}
