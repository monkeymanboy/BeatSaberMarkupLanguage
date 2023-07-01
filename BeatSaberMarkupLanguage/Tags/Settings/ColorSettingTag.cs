using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Util;
using HMUI;
using Polyglot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class ColorSettingTag : ModalColorPickerTag
    {
        private FormattedFloatListSettingsValueController baseSettings;
        private ImageView colorImage;

        public override string[] Aliases => new[] { "color-setting" };

        public override void Setup()
        {
            base.Setup();
            baseSettings = DiContainer.Resolve<SettingsNavigationController>().GetComponentOnChild<FormattedFloatListSettingsValueController>("GraphicSettings/ViewPort/Content/VRRenderingScale");
            colorImage = DiContainer.Resolve<GameplaySetupViewController>().GetComponentOnChild<ImageView>("ColorsOverrideSettings/Settings/Detail/ColorSchemeDropDown/DropDownButton/ColorSchemeView/SaberColorA");
        }

        public override GameObject CreateObject(Transform parent)
        {
            FormattedFloatListSettingsValueController baseSetting = Object.Instantiate(baseSettings, parent, false);
            baseSetting.name = "BSMLColorSetting";

            GameObject gameObject = baseSetting.gameObject;
            gameObject.SetActive(false);

            Object.Destroy(baseSetting);
            ColorSetting colorSetting = gameObject.AddComponent<ColorSetting>();

            Transform valuePick = gameObject.transform.Find("ValuePicker");
            (valuePick.transform as RectTransform).sizeDelta = new Vector2(13, 0);

            Object.Destroy(valuePick.transform.Find("DecButton").gameObject);

            Transform editButtonTransform = valuePick.transform.Find("IncButton");
            editButtonTransform.name = "EditButton";

            Object.Destroy(valuePick.GetComponentsInChildren<TextMeshProUGUI>().First().gameObject);
            colorSetting.editButton = editButtonTransform.GetComponent<Button>();

            GameObject nameText = gameObject.transform.Find("NameText").gameObject;
            LocalizedTextMeshProUGUI localizedText = ConfigureLocalizedText(nameText);

            TextMeshProUGUI text = nameText.GetComponent<TextMeshProUGUI>();
            text.text = "Default Text";

            List<Component> externalComponents = gameObject.AddComponent<ExternalComponents>().components;
            externalComponents.Add(text);
            externalComponents.Add(localizedText);

            gameObject.GetComponent<LayoutElement>().preferredWidth = 90;

            Image instance = Object.Instantiate(colorImage, valuePick, false);
            instance.name = "BSMLCurrentColor";
            RectTransform imageTransform = (RectTransform)instance.transform;
            imageTransform.anchoredPosition = new Vector2(0, 0);
            imageTransform.sizeDelta = new Vector2(5, 5);
            imageTransform.anchorMin = new Vector2(0.25f, 0.5f);
            imageTransform.anchorMax = new Vector2(0.25f, 0.5f);
            colorSetting.colorImage = instance;

            Image icon = colorSetting.editButton.transform.Find("Icon").GetComponent<Image>();
            icon.name = "EditIcon";
            icon.sprite = Utilities.EditIcon;
            icon.rectTransform.sizeDelta = new Vector2(4, 4);
            colorSetting.editButton.interactable = true;

            ((RectTransform)colorSetting.editButton.transform).anchorMin = new Vector2(0, 0);

            colorSetting.modalColorPicker = base.CreateObject(gameObject.transform).GetComponent<ModalColorPicker>();

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
