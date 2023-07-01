using System.Collections.Generic;
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
    public class StringSettingTag : ModalKeyboardTag
    {
        private FormattedFloatListSettingsValueController valueControllerTemplate;

        public override string[] Aliases => new[] { "string-setting" };

        public override void Setup()
        {
            base.Setup();
            valueControllerTemplate = DiContainer.Resolve<SettingsNavigationController>().GetComponentOnChild<FormattedFloatListSettingsValueController>("GraphicSettings/ViewPort/Content/VRRenderingScale");
        }

        public override GameObject CreateObject(Transform parent)
        {
            FormattedFloatListSettingsValueController baseSetting = Object.Instantiate(valueControllerTemplate, parent, false);
            baseSetting.name = "BSMLStringSetting";

            GameObject gameObject = baseSetting.gameObject;
            gameObject.SetActive(false);

            Object.Destroy(baseSetting);

            Transform valuePickerTransform = gameObject.transform.Find("ValuePicker");
            Object.Destroy(valuePickerTransform.GetComponent<StepValuePicker>());
            Object.Destroy(valuePickerTransform.Find("DecButton").gameObject);

            Transform editButtonTransform = valuePickerTransform.Find("IncButton");
            editButtonTransform.name = "EditButton";

            // TODO: this button has a gradient and simply disabling the gradient breaks colors - seems to be related to button animations
            ImageView buttonBackgroundImageView = editButtonTransform.Find("BG").GetComponent<ImageView>();
            buttonBackgroundImageView.sprite = Utilities.FindSpriteCached("RoundRect10Thin");

            RectTransform valueTextTransform = (RectTransform)valuePickerTransform.Find("ValueText");
            valueTextTransform.offsetMin = new Vector2(2, 0);

            TextMeshProUGUI valueText = valueTextTransform.GetComponent<TextMeshProUGUI>();
            valueText.overflowMode = TextOverflowModes.Ellipsis;
            valueText.richText = true;

            StringSetting stringSetting = gameObject.AddComponent<StringSetting>();
            stringSetting.text = valueText;
            stringSetting.editButton = editButtonTransform.GetComponent<Button>();
            stringSetting.boundingBox = (RectTransform)valuePickerTransform;

            GameObject nameText = gameObject.transform.Find("NameText").gameObject;
            LocalizedTextMeshProUGUI localizedText = ConfigureLocalizedText(nameText);

            TextMeshProUGUI text = nameText.GetComponent<TextMeshProUGUI>();
            text.text = "Default Text";

            List<Component> externalComponents = gameObject.AddComponent<ExternalComponents>().components;
            externalComponents.Add(text);
            externalComponents.Add(localizedText);

            gameObject.GetComponent<LayoutElement>().preferredWidth = 90;
            stringSetting.text.alignment = TextAlignmentOptions.MidlineRight;
            stringSetting.text.enableWordWrapping = false;

            Image icon = stringSetting.editButton.transform.Find("Icon").GetComponent<Image>();
            icon.name = "EditIcon";
            icon.sprite = Utilities.EditIcon;
            icon.rectTransform.sizeDelta = new Vector2(4, 4);
            stringSetting.editButton.interactable = true;

            ((RectTransform)stringSetting.editButton.transform).anchorMin = new Vector2(0, 0);

            stringSetting.modalKeyboard = base.CreateObject(gameObject.transform).GetComponent<ModalKeyboard>();

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
