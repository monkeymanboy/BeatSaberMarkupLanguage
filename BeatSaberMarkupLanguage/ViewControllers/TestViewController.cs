using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.ViewControllers
{
    public class TestViewController : BSMLResourceViewController
    {
        public override string ResourceName => "BeatSaberMarkupLanguage.Views.test.bsml";

        [UIComponent("sometext")]
        public TextMeshProUGUI text;

        [UIComponent("list")]
        public CustomListTableData tableData;

        [UIAction("click")]
        private void ButtonPress()
        {
            text.text = "It works!";
        }

        [UIAction("cell click")]
        private void CellClick(TableView tableView, int index)
        {
            text.text = tableData.data[index].text;
        }
    }
}
