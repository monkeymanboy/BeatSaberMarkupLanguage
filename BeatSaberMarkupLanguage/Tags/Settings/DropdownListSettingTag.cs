using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
using Polyglot;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class DropdownListSettingTag : BSMLTag
    {
        private GameObject dropdownTemplate;

        public override string[] Aliases => new[] { "dropdown-list-setting" };

        private GameObject safePrefab;
        public override void Setup()
        {
            if (dropdownTemplate == null)
                dropdownTemplate = Resources.FindObjectsOfTypeAll<SimpleTextDropdown>().First(x => x.transform?.parent?.name == "NormalLevels").transform.parent.gameObject;
            safePrefab = Object.Instantiate(dropdownTemplate, BSMLParser.instance.transform, false);
            safePrefab.SetActive(false);
            safePrefab.name = "BSMLDropdownListPrefab";
        }

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = diContainer.InstantiatePrefab(safePrefab, parent);
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
