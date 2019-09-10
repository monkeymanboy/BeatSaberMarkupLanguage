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
            GameObject.Destroy(gameObject.transform.GetChild(1).GetComponentsInChildren<Button>().First().gameObject);
            stringSetting.text = gameObject.transform.GetChild(1).GetComponentsInChildren<TextMeshProUGUI>().First();
            stringSetting.label = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            stringSetting.editButton = gameObject.transform.GetChild(1).GetComponentsInChildren<Button>().Last();

            MonoBehaviour.Destroy(stringSetting.label.GetComponent<LocalizedTextMeshProUGUI>());
            gameObject.GetComponent<LayoutElement>().preferredWidth = 90;
            stringSetting.text.alignment = TextAlignmentOptions.MidlineRight;
            stringSetting.text.enableWordWrapping = false;
            stringSetting.LabelText = "Default Text";
            stringSetting.Text = "Default Text";

            Image icon = stringSetting.editButton.transform.Find("Arrow").GetComponent<Image>();
            icon.name = "EditIcon";
            icon.sprite = Utilities.EditIcon;
            icon.rectTransform.sizeDelta = new Vector2(4, 4);
            stringSetting.editButton.interactable = true;

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
