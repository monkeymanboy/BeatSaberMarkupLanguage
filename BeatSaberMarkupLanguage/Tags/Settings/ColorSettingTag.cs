using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
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

            Object.Destroy(valuePick.transform.Find("DecButton").gameObject);

            Transform editButtonTransform = valuePick.transform.Find("IncButton");
            editButtonTransform.name = "EditButton";

            // TODO: this button has a gradient and simply disabling the gradient breaks colors - seems to be related to button animations
            ImageView buttonBackgroundImageView = editButtonTransform.Find("BG").GetComponent<ImageView>();
            buttonBackgroundImageView.sprite = Utilities.FindSpriteCached("RoundRect10Thin");

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

            if (colorImage == null)
            {
                colorImage = Resources.FindObjectsOfTypeAll<Image>().Where(i => i.sprite != null).First(i => i.gameObject.name == "ColorImage" && i.sprite.name == "NoteCircle");
            }

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
