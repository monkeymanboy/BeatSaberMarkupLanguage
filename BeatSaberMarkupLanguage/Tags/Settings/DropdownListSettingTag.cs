using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BGLib.Polyglot;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class DropdownListSettingTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "dropdown-list-setting" };

        protected override PrefabParams CreatePrefab()
        {
            GameObject gameObject = BeatSaberUI.DiContainer.InstantiatePrefab(BeatSaberUI.DiContainer.Resolve<GameplaySetupViewController>()._environmentOverrideSettingsPanelController._elementsGO.transform.Find("NormalLevels").gameObject);
            gameObject.name = "BSMLDropdownList";
            SimpleTextDropdown dropdown = gameObject.GetComponentInChildren<SimpleTextDropdown>();
            GameObject dropdownGameObject = dropdown.gameObject;
            dropdownGameObject.SetActive(false);
            dropdown.name = "Dropdown";

            GameObject labelObject = gameObject.transform.Find("Label").gameObject;
            LocalizedTextMeshProUGUI localizedText = ConfigureLocalizedText(labelObject);

            CurvedTextMeshPro textMesh = labelObject.GetComponent<CurvedTextMeshPro>();
            textMesh.text = "Default Text";

            LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 8;
            layoutElement.preferredWidth = 90;

            List<Component> externalComponents = dropdownGameObject.AddComponent<ExternalComponents>().components;
            externalComponents.Add(textMesh);
            externalComponents.Add(localizedText);
            externalComponents.Add(layoutElement);

            DropDownListSetting dropDownListSetting = dropdownGameObject.AddComponent<DropDownListSetting>();

            dropDownListSetting.dropdown = dropdown;
            return new PrefabParams(gameObject, dropdownGameObject);
        }
    }
}
