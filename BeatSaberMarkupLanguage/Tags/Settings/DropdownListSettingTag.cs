using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BGLib.Polyglot;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class DropdownListSettingTag : BSMLTag
    {
        private GameObject dropdownTemplate;
        private GameObject safePrefab;

        public override string[] Aliases => new[] { "dropdown-list-setting" };

        public override void Setup()
        {
            if (dropdownTemplate == null)
            {
                dropdownTemplate = DiContainer.Resolve<GameplaySetupViewController>()._environmentOverrideSettingsPanelController._elementsGO.transform.Find("NormalLevels").gameObject;
            }

            safePrefab = Object.Instantiate(dropdownTemplate);
            safePrefab.SetActive(false);
            safePrefab.name = "BSMLDropdownListPrefab";
            Object.DontDestroyOnLoad(safePrefab);
        }

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = DiContainer.InstantiatePrefab(safePrefab, parent);
            gameObject.name = "BSMLDropdownList";
            SimpleTextDropdown dropdown = gameObject.GetComponentInChildren<SimpleTextDropdown>();
            dropdown.gameObject.SetActive(false);
            dropdown.name = "Dropdown";

            GameObject labelObject = gameObject.transform.Find("Label").gameObject;
            LocalizedTextMeshProUGUI localizedText = ConfigureLocalizedText(labelObject);

            CurvedTextMeshPro textMesh = labelObject.GetComponent<CurvedTextMeshPro>();
            textMesh.text = "Default Text";

            LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 8;
            layoutElement.preferredWidth = 90;

            List<Component> externalComponents = dropdown.gameObject.AddComponent<ExternalComponents>().components;
            externalComponents.Add(textMesh);
            externalComponents.Add(localizedText);
            externalComponents.Add(layoutElement);

            DropDownListSetting dropDownListSetting = dropdown.gameObject.AddComponent<DropDownListSetting>();

            dropDownListSetting.dropdown = dropdown;
            dropdown.gameObject.SetActive(true);
            gameObject.SetActive(true);
            return dropdown.gameObject;
        }
    }
}
