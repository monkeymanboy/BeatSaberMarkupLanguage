using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
using IPA.Utilities;
using Polyglot;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class DropdownListSettingTag : BSMLTag
    {
        public override string[] Aliases => new[] { "dropdown-list-setting" };

        private GameObject safePrefab;
        public override void Setup()
        {
            safePrefab = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<SimpleTextDropdown>().First(x => x.transform?.parent?.name == "NormalLevels").transform.parent.gameObject, BSMLParser.instance.transform, false);
            safePrefab.SetActive(false);
            safePrefab.name = "BSMLDropdownListPrefab";
        }

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = MonoBehaviour.Instantiate(safePrefab, parent, false);
            gameObject.name = "BSMLDropdownList";
            SimpleTextDropdown dropdown = gameObject.GetComponentInChildren<SimpleTextDropdown>();
            dropdown.gameObject.SetActive(false);
            dropdown.name = "Dropdown";
            dropdown.GetComponentInChildren<VRGraphicRaycaster>(true).SetField("_physicsRaycaster", BeatSaberUI.PhysicsRaycasterWithCache);
            dropdown.GetComponentInChildren<ModalView>(true).SetField("_container", BeatSaberUI.DiContainer);

            ExternalComponents externalComponents = dropdown.gameObject.AddComponent<ExternalComponents>();

            GameObject labelObject = gameObject.transform.Find("Label").gameObject;
            MonoBehaviour.Destroy(labelObject.GetComponent<LocalizedTextMeshProUGUI>());
            externalComponents.components.Add(gameObject.transform.Find("Label").GetComponent<CurvedTextMeshPro>());

            LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 8;
            layoutElement.preferredWidth = 90;
            externalComponents.components.Add(layoutElement);

            DropDownListSetting dropDownListSetting = dropdown.gameObject.AddComponent<DropDownListSetting>();
            
            dropDownListSetting.dropdown = dropdown;
            dropdown.gameObject.SetActive(true);
            gameObject.SetActive(true);
            return dropdown.gameObject;
        }
    }
}
