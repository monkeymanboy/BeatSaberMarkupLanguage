using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using Polyglot;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class StringSettingTag : ModalKeyboardTag
    {
        public override string[] Aliases => new[] { "string-setting" };

        public override GameObject CreateObject(Transform parent)
        {
            FormattedFloatListSettingsValueController baseSetting = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<FormattedFloatListSettingsValueController>().First(x => (x.name == "VRRenderingScale")), parent, false);
            baseSetting.name = "BSMLStringSetting";

            GameObject gameObject = baseSetting.gameObject;
            gameObject.SetActive(false);

            MonoBehaviour.Destroy(baseSetting);
            StringSetting stringSetting = gameObject.AddComponent<StringSetting>();
            Transform valuePick = gameObject.transform.Find("ValuePicker");
            MonoBehaviour.Destroy(valuePick.GetComponent<StepValuePicker>());
            Button decButton = valuePick.GetComponentsInChildren<Button>().First();
            decButton.enabled = false;
            decButton.interactable = true;
            GameObject.Destroy(decButton.transform.Find("Icon").gameObject);
            stringSetting.text = valuePick.GetComponentsInChildren<TextMeshProUGUI>().First();
            stringSetting.text.richText = true;
            stringSetting.editButton = valuePick.GetComponentsInChildren<Button>().Last();
            stringSetting.boundingBox = valuePick as RectTransform;

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
            RectTransform textTransform = stringSetting.text.transform as RectTransform;
            textTransform.anchorMin = new Vector2(0, 0);
            textTransform.anchorMax = new Vector2(1, 1);
            textTransform.anchoredPosition = new Vector2(-6, 0);

            Image icon = stringSetting.editButton.transform.Find("Icon").GetComponent<Image>();
            icon.name = "EditIcon";
            icon.sprite = Utilities.EditIcon;
            icon.rectTransform.sizeDelta = new Vector2(4, 4);
            stringSetting.editButton.interactable = true;
            
            (stringSetting.editButton.transform as RectTransform).anchorMin = new Vector2(0, 0);

            stringSetting.modalKeyboard = base.CreateObject(gameObject.transform).GetComponent<ModalKeyboard>();

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
