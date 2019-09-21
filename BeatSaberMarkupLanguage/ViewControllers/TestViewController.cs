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
                list.Add(new TestListObject("first"));
                list.Add(new TestListObject("second"));
                list.Add(new TestListObject("third"));
                list.Add(new TestListObject("fourth"));
                list.Add(new TestListObject("fifth"));
                list.Add(new TestListObject("sixth"));
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
        public TestListObject(string title)
        {
            this.title = title;
        }

        [UIAction("button-click")]
        void ClickedButton()
        {
            Logger.log.Debug("Button - " + title);
        }
    }
}
