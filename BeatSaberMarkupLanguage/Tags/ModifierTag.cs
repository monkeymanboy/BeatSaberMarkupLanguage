using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ModifierTag : BSMLTag
    {
        public override string[] Aliases => new[] { "modifier" };

        public override GameObject CreateObject(Transform parent)
        {
            GameplayModifierToggle baseModifier = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<GameplayModifierToggle>().First(x => (x.name == "InstaFail")), parent, false);
            baseModifier.name = "BSMLModifier";

            GameObject gameObject = baseModifier.gameObject;

            MonoBehaviour.Destroy(baseModifier);
            MonoBehaviour.Destroy(gameObject.GetComponent<HoverHint>());
            
            ExternalComponents externalComponents = gameObject.AddComponent<ExternalComponents>();
            externalComponents.components.Add(gameObject.GetComponentInChildren<TextMeshProUGUI>());
            externalComponents.components.Add(gameObject.transform.Find("Icon").GetComponent<Image>());

            /*
            CheckboxSetting checkboxSetting = gameObject.AddComponent<CheckboxSetting>();
            checkboxSetting.checkbox = gameObject.GetComponent<Toggle>();
            */

            return gameObject;
        }
    }
}
