using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
using IPA.Utilities;
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

            GameObject.Destroy(gameObject.transform.Find("Label").GetComponent<CurvedTextMeshPro>().gameObject);
            
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(gameObject.transform, false);

            CurvedTextMeshPro textMesh = labelObj.AddComponent<CurvedTextMeshPro>();
            textMesh.font = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<TMP_FontAsset>().First(t => t.name == "Teko-Medium SDF No Glow"));
            textMesh.fontSize = 4;
            textMesh.fontStyle = FontStyles.Italic;
            textMesh.alignment = TextAlignmentOptions.Left;
            textMesh.color = Color.white;

            textMesh.rectTransform.anchoredPosition = new Vector2(0, 0);
            textMesh.rectTransform.anchorMin = new Vector2(0, 0);
            textMesh.rectTransform.anchorMax = new Vector2(1, 1);
            textMesh.rectTransform.sizeDelta = new Vector2(0, 0);
            ExternalComponents externalComponents = dropdown.gameObject.AddComponent<ExternalComponents>();
            externalComponents.components.Add(textMesh);

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
