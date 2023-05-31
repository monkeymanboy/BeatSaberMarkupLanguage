using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using Polyglot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class ColorSettingTag : ModalColorPickerTag
    {
        private static FormattedFloatListSettingsValueController baseSettings;
        private static Image colorImage;

        public override string[] Aliases => new[] { "color-setting" };

        public override GameObject CreateObject(Transform parent)
        {
            if (baseSettings == null)
            {
                baseSettings = Resources.FindObjectsOfTypeAll<FormattedFloatListSettingsValueController>().First(x => x.name == "VRRenderingScale");
            }

            FormattedFloatListSettingsValueController baseSetting = Object.Instantiate(baseSettings, parent, false);
            baseSetting.name = "BSMLColorSetting";

            GameObject gameObject = baseSetting.gameObject;
            gameObject.SetActive(false);

            Object.Destroy(baseSetting);
            ColorSetting colorSetting = gameObject.AddComponent<ColorSetting>();

            Transform valuePick = gameObject.transform.Find("ValuePicker");
            (valuePick.transform as RectTransform).sizeDelta = new Vector2(13, 0);

            Button decButton = valuePick.GetComponentsInChildren<Button>().First();
            decButton.enabled = false;
            decButton.interactable = true;
            Object.Destroy(decButton.transform.Find("Icon").gameObject);
            Object.Destroy(valuePick.GetComponentsInChildren<TextMeshProUGUI>().First().gameObject);
            colorSetting.editButton = valuePick.GetComponentsInChildren<Button>().Last();

            GameObject nameText = gameObject.transform.Find("NameText").gameObject;
            LocalizedTextMeshProUGUI localizedText = ConfigureLocalizedText(nameText);

            TextMeshProUGUI text = nameText.GetComponent<TextMeshProUGUI>();
            text.text = "Default Text";

            List<Component> externalComponents = gameObject.AddComponent<ExternalComponents>().components;
            externalComponents.Add(text);
            externalComponents.Add(localizedText);

            gameObject.GetComponent<LayoutElement>().preferredWidth = 90;

            if (colorImage == null)
            {
                colorImage = Resources.FindObjectsOfTypeAll<Image>().First(x => x.gameObject.name == "ColorImage" && x.sprite?.name == "NoteCircle");
            }

            Image instance = Object.Instantiate(colorImage, valuePick, false);
            instance.name = "BSMLCurrentColor";
            (instance.gameObject.transform as RectTransform).anchoredPosition = new Vector2(0, 0);
            (instance.gameObject.transform as RectTransform).sizeDelta = new Vector2(5, 5);
            (instance.gameObject.transform as RectTransform).anchorMin = new Vector2(0.2f, 0.5f);
            (instance.gameObject.transform as RectTransform).anchorMax = new Vector2(0.2f, 0.5f);
            colorSetting.colorImage = instance;

            Image icon = colorSetting.editButton.transform.Find("Icon").GetComponent<Image>();
            icon.name = "EditIcon";
            icon.sprite = Utilities.EditIcon;
            icon.rectTransform.sizeDelta = new Vector2(4, 4);
            colorSetting.editButton.interactable = true;

            (colorSetting.editButton.transform as RectTransform).anchorMin = new Vector2(0, 0);

            colorSetting.modalColorPicker = base.CreateObject(gameObject.transform).GetComponent<ModalColorPicker>();

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
