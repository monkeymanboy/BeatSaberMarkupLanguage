using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using Polyglot;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class StringSettingTag : BSMLTag
    {
        public override string[] Aliases => new[] { "string-setting" };

        public override GameObject CreateObject(Transform parent)
        {
            BoolSettingsController baseSetting = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<BoolSettingsController>().First(x => (x.name == "Fullscreen")), parent, false);
            baseSetting.name = "BSMLStringSetting";

            GameObject gameObject = baseSetting.gameObject;
            gameObject.SetActive(false);

            MonoBehaviour.Destroy(baseSetting);
            StringSetting stringSetting = gameObject.AddComponent<StringSetting>();
            Transform valuePick = gameObject.transform.Find("ValuePicker");
            Button decButton = valuePick.GetComponentsInChildren<Button>().First();
            decButton.enabled = false;
            GameObject.Destroy(decButton.transform.Find("Arrow").gameObject);
            stringSetting.text = valuePick.GetComponentsInChildren<TextMeshProUGUI>().First();
            stringSetting.editButton = valuePick.GetComponentsInChildren<Button>().Last();
            stringSetting.boundingBox = valuePick as RectTransform;
            
            TextMeshProUGUI text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            text.text = "Default Text";
            gameObject.AddComponent<ExternalComponents>().components.Add(text);
            MonoBehaviour.Destroy(text.GetComponent<LocalizedTextMeshProUGUI>());
            
            gameObject.GetComponent<LayoutElement>().preferredWidth = 90;
            stringSetting.text.alignment = TextAlignmentOptions.MidlineRight;
            stringSetting.text.enableWordWrapping = false;

            Image icon = stringSetting.editButton.transform.Find("Arrow").GetComponent<Image>();
            icon.name = "EditIcon";
            icon.sprite = Utilities.EditIcon;
            icon.rectTransform.sizeDelta = new Vector2(4, 4);
            stringSetting.editButton.interactable = true;

            (stringSetting.editButton.transform as RectTransform).anchorMin = new Vector2(0, 0);

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
