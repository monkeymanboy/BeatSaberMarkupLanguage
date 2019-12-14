using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Notify;
using HMUI;
using System.Collections.Generic;
using TMPro;

namespace BeatSaberMarkupLanguage.ViewControllers
{
    public class TestViewController : BSMLResourceViewController, INotifiableHost
    {
        public override string ResourceName => "BeatSaberMarkupLanguage.Views.test.bsml";

        [UIValue("header")]
        public string HeaderText
        {
            get => headerText;
            set
            {
                headerText = value;
                NotifyPropertyChanged();
            }
        }
        public string headerText = "Header comes from code!";

        [UIComponent("test-external")]
        public TextMeshProUGUI buttonText;

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
                return list;
            }
        }

        [UIAction("click")]
        private void ButtonPress()
        {
            HeaderText = "It works!";
            buttonText.text = "Clicked";
        }

        [UIAction("cell click")]
        private void CellClick(TableView tableView, TestListObject testObj)
        {
            Logger.log.Info("Clicked - " + testObj.title);
        }

        [UIAction("keyboard-enter")]
        private void KeyboardEnter(string value)
        {
            Logger.log.Info("Keyboard typed: " + value);
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
            Logger.log.Info("Button - " + title);
        }
    }
}
