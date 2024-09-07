using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BGLib.Polyglot;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ToggleSettingTag : BSMLTag
    {
        private GameObject toggleTemplate;

        public override string[] Aliases => new[] { "toggle-setting", "bool-setting", "checkbox-setting", "checkbox" };

        public override GameObject CreateObject(Transform parent)
        {
            if (toggleTemplate == null)
            {
                toggleTemplate = DiContainer.Resolve<MainSettingsMenuViewController>()._settingsSubMenuInfos.Select(m => m.viewController).First(vc => vc.name == "GraphicSettings").transform.Find("ViewPort/Content/Fullscreen").gameObject;
            }

            GameObject gameObject = Object.Instantiate(toggleTemplate, parent, false);
            GameObject nameText = gameObject.transform.Find("NameText").gameObject;
            GameObject switchView = gameObject.transform.Find("SwitchView").gameObject;
            Object.Destroy(gameObject.GetComponent<SwitchSettingsController>());

            gameObject.name = "BSMLToggle";
            gameObject.SetActive(false);

            ToggleSetting toggleSetting = gameObject.AddComponent<ToggleSetting>();
            AnimatedSwitchView animatedSwitchView = switchView.GetComponent<AnimatedSwitchView>();

            toggleSetting.Toggle = switchView.GetComponent<Toggle>();
            toggleSetting.Toggle.onValueChanged.RemoveAllListeners();
            toggleSetting.Toggle.onValueChanged.AddListener(animatedSwitchView.HandleOnValueChanged);
            toggleSetting.Toggle.interactable = true;
            toggleSetting.Toggle.isOn = false;
            animatedSwitchView.enabled = true; // force refresh the UI state

            LocalizedTextMeshProUGUI localizedText = ConfigureLocalizedText(nameText);

            toggleSetting.TextMesh = nameText.GetComponent<TextMeshProUGUI>();
            toggleSetting.TextMesh.text = "Default Text";
            toggleSetting.TextMesh.richText = true;
            toggleSetting.TextMesh.overflowMode = TextOverflowModes.Ellipsis;

            List<Component> externalComponents = gameObject.AddComponent<ExternalComponents>().Components;
            externalComponents.Add(toggleSetting.TextMesh);
            externalComponents.Add(localizedText);

            gameObject.GetComponent<LayoutElement>().preferredWidth = 90;

            gameObject.SetActive(true);

            return gameObject;
        }
    }
}
