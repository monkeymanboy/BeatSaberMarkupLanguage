using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using Polyglot;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class CheckboxSettingTag : BSMLTag
    {
        public override string[] Aliases => new[] { "checkbox-setting", "checkbox" };

        public override GameObject CreateObject(Transform parent)
        {
            GameplayModifierToggle baseSetting = Object.Instantiate(Resources.FindObjectsOfTypeAll<GameplayModifierToggle>().First(x => x.name == "InstaFail"), parent, false);
            baseSetting.name = "BSMLCheckboxSetting";

            GameObject gameObject = baseSetting.gameObject;
            gameObject.SetActive(false);

            Object.Destroy(baseSetting);
            Object.Destroy(gameObject.transform.GetChild(0).gameObject); //Remove icon
            Object.Destroy(gameObject.GetComponent<SignalOnUIToggleValueChanged>()); //Remove base game signal
            Object.Destroy(gameObject.GetComponent<HoverHint>()); //When parsing, "RectTransform" will already add a new one. No need for this.
            CheckboxSetting checkboxSetting = gameObject.AddComponent<CheckboxSetting>();
            checkboxSetting.checkbox = gameObject.GetComponent<Toggle>();
            TextMeshProUGUI text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            text.fontSize = 5; //Change some settings to conform more to the List Dropdown/IncDec settings controllers
            text.rectTransform.localPosition = Vector2.zero;
            text.rectTransform.anchoredPosition = Vector2.zero;
            text.rectTransform.sizeDelta = Vector2.zero;
            gameObject.AddComponent<ExternalComponents>().components.Add(text);

            LayoutElement layout = gameObject.GetComponent<LayoutElement>(); //If Beat Games decides to add one later down the road.
            if (layout is null) layout = gameObject.AddComponent<LayoutElement>(); //For the time being, they dont have one, so time to add one myself!
            layout.preferredWidth = 90; //Again, to conform to List Dropdown/IncDec settings controllers
            layout.preferredHeight = 8;

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
