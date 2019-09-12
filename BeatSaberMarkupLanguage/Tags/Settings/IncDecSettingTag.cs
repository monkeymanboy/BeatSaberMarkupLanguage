using BeatSaberMarkupLanguage.Components.Settings;
using Polyglot;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public abstract class IncDecSettingTag<T> : BSMLTag where T : IncDecSetting
    {
        public override GameObject CreateObject(Transform parent)
        {
            BoolSettingsController baseSetting = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<BoolSettingsController>().First(x => (x.name == "Fullscreen")), parent, false);
            baseSetting.name = "BSMLBoolSetting";

            GameObject gameObject = baseSetting.gameObject;
            MonoBehaviour.Destroy(baseSetting);
            gameObject.SetActive(false);

            T boolSetting = gameObject.AddComponent<T>();
            boolSetting.text = gameObject.transform.GetChild(1).GetComponentsInChildren<TextMeshProUGUI>().First();
            boolSetting.decButton = gameObject.transform.GetChild(1).GetComponentsInChildren<Button>().First();
            boolSetting.incButton = gameObject.transform.GetChild(1).GetComponentsInChildren<Button>().Last();
            boolSetting.label = gameObject.GetComponentInChildren<TextMeshProUGUI>();

            MonoBehaviour.Destroy(boolSetting.label.GetComponent<LocalizedTextMeshProUGUI>());
            gameObject.GetComponent<LayoutElement>().preferredWidth = 90;
            boolSetting.LabelText = "Default Text";

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
