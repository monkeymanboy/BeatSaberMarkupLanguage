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
    public class StringSettingTag : ModalKeyboardTag
    {
        private FormattedFloatListSettingsValueController valueControllerTemplate;

        public override string[] Aliases => new[] { "string-setting" };

        public override GameObject CreateObject(Transform parent)
        {
            if (valueControllerTemplate == null)
            {
                valueControllerTemplate = Resources.FindObjectsOfTypeAll<FormattedFloatListSettingsValueController>().First(x => x.name == "VRRenderingScale");
            }

            FormattedFloatListSettingsValueController baseSetting = Object.Instantiate(valueControllerTemplate, parent, false);
            baseSetting.name = "BSMLStringSetting";

            GameObject gameObject = baseSetting.gameObject;
            gameObject.SetActive(false);

            Object.Destroy(baseSetting);
            StringSetting stringSetting = gameObject.AddComponent<StringSetting>();
            Transform valuePick = gameObject.transform.Find("ValuePicker");
            Object.Destroy(valuePick.GetComponent<StepValuePicker>());
            Object.Destroy(valuePick.transform.Find("DecButton").gameObject);

            Transform editButtonTransform = valuePick.transform.Find("IncButton");
            editButtonTransform.name = "EditButton";

            // TODO: this button has a gradient and simply disabling the gradient breaks colors - seems to be related to button animations
            ImageView buttonBackgroundImageView = editButtonTransform.Find("BG").GetComponent<ImageView>();
            buttonBackgroundImageView.sprite = Utilities.FindSpriteCached("RoundRect10Thin");

            stringSetting.text = valuePick.GetComponentsInChildren<TextMeshProUGUI>().First();
            stringSetting.text.richText = true;
            stringSetting.editButton = editButtonTransform.GetComponent<Button>();
            stringSetting.boundingBox = (RectTransform)valuePick;

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
            RectTransform textTransform = (RectTransform)stringSetting.text.transform;
            textTransform.anchorMin = new Vector2(0, 0);
            textTransform.anchorMax = new Vector2(1, 1);
            textTransform.anchoredPosition = new Vector2(-6, 0);

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
