using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BGLib.Polyglot;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class StringSettingTag : ModalKeyboardTag
    {
        private FormattedFloatListSettingsController valueControllerTemplate;

        public override string[] Aliases => new[] { "string-setting" };

        public override GameObject CreateObject(Transform parent)
        {
            if (valueControllerTemplate == null)
            {
                valueControllerTemplate = DiContainer.Resolve<MainSettingsMenuViewController>()._settingsSubMenuInfos.Select(m => m.viewController).First(vc => vc.name == "GraphicSettings").transform.Find("ViewPort/Content/VRRenderingScale").GetComponent<FormattedFloatListSettingsController>();
            }

            FormattedFloatListSettingsController baseSetting = Object.Instantiate(valueControllerTemplate, parent, false);
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
            stringSetting.TextMesh = valueText;
            stringSetting.EditButton = editButtonTransform.GetComponent<Button>();
            stringSetting.BoundingBox = (RectTransform)valuePickerTransform;

            GameObject nameText = gameObject.transform.Find("NameText").gameObject;
            LocalizedTextMeshProUGUI localizedText = ConfigureLocalizedText(nameText);

            TextMeshProUGUI text = nameText.GetComponent<TextMeshProUGUI>();
            text.text = "Default Text";

            List<Component> externalComponents = gameObject.AddComponent<ExternalComponents>().Components;
            externalComponents.Add(text);
            externalComponents.Add(localizedText);

            gameObject.GetComponent<LayoutElement>().preferredWidth = 90;
            stringSetting.TextMesh.alignment = TextAlignmentOptions.MidlineRight;
            stringSetting.TextMesh.enableWordWrapping = false;

            Image icon = stringSetting.EditButton.transform.Find("Icon").GetComponent<Image>();
            icon.name = "EditIcon";
            icon.sprite = Utilities.EditIcon;
            icon.rectTransform.sizeDelta = new Vector2(4, 4);
            stringSetting.EditButton.interactable = true;

            ((RectTransform)stringSetting.EditButton.transform).anchorMin = new Vector2(0, 0);

            stringSetting.ModalKeyboard = base.CreateObject(gameObject.transform).GetComponent<ModalKeyboard>();

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
