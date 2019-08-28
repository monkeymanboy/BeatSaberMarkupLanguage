using BeatSaberMarkupLanguage.Components.Settings;
using BS_Utils.Utilities;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class DropdownListSettingTag : BSMLTag
    {
        public override string[] Aliases => new[] { "dropdown-list-setting" };
        public override GameObject CreateObject(Transform parent)
        {
            LabelAndValueDropdownWithTableView dropdown = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<LabelAndValueDropdownWithTableView>().First(), parent, false);
            dropdown.gameObject.SetActive(false);
            dropdown.name = "BSMLDropDownList";
            dropdown.GetPrivateField<TextMeshProUGUI>("_labelText").fontSize = 5;
            LayoutElement layoutElement = dropdown.gameObject.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 8;
            layoutElement.preferredWidth = 90;
            dropdown.SetLabelText("Default Text");
            dropdown.SetValueText("Default Text");
            DropDownListSetting dropDownListSetting = dropdown.gameObject.AddComponent<DropDownListSetting>();
            dropDownListSetting.tableView = dropdown.GetPrivateField<TableView>("_tableView");
            dropDownListSetting.dropdown = dropdown;
            dropdown.GetPrivateField<TableView>("_tableView").dataSource = dropDownListSetting;
            return dropdown.gameObject;
        }
    }
}
