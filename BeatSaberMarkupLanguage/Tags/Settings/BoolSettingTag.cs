using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class BoolSettingTag : BSMLTag
    {
        public override string[] Aliases => new[] { "bool-setting" };

        public override GameObject CreateObject(Transform parent)
        {
            BoolSettingsController baseSetting = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<BoolSettingsController>().First(x => (x.name == "Fullscreen")), parent, false);
            baseSetting.name = "BSMLBoolSetting";
            GameObject gameObject = baseSetting.gameObject;
            MonoBehaviour.Destroy(baseSetting);
            gameObject.SetActive(false);
            BoolSetting boolSetting = gameObject.AddComponent<BoolSetting>();
            boolSetting.text = gameObject.transform.GetChild(1).GetComponentsInChildren<TextMeshProUGUI>().First();
            boolSetting.decButton = gameObject.transform.GetChild(1).GetComponentsInChildren<Button>().First();
            boolSetting.incButton = gameObject.transform.GetChild(1).GetComponentsInChildren<Button>().Last();
            boolSetting.label = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            boolSetting.LabelText = "Default Text";
            gameObject.SetActive(true);
            return gameObject;
        }

    }
}
