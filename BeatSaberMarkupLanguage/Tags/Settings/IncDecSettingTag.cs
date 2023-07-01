using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Util;
using Polyglot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public abstract class IncDecSettingTag<T> : BSMLTag
        where T : IncDecSetting
    {
        private FormattedFloatListSettingsValueController valueControllerTemplate;

        public override void Setup()
        {
            base.Setup();
            valueControllerTemplate = DiContainer.Resolve<SettingsNavigationController>().GetComponentOnChild<FormattedFloatListSettingsValueController>("GraphicSettings/ViewPort/Content/VRRenderingScale");
        }

        public override GameObject CreateObject(Transform parent)
        {
            FormattedFloatListSettingsValueController baseSetting = Object.Instantiate(valueControllerTemplate, parent, false);
            baseSetting.name = "BSMLIncDecSetting";

            GameObject gameObject = baseSetting.gameObject;
            Object.Destroy(baseSetting);
            gameObject.SetActive(false);

            T boolSetting = gameObject.AddComponent<T>();
            boolSetting.text = gameObject.transform.GetChild(1).GetComponentsInChildren<TextMeshProUGUI>().First();
            boolSetting.text.richText = true;
            boolSetting.decButton = gameObject.transform.GetChild(1).GetComponentsInChildren<Button>().First();
            boolSetting.incButton = gameObject.transform.GetChild(1).GetComponentsInChildren<Button>().Last();
            (gameObject.transform.GetChild(1) as RectTransform).sizeDelta = new Vector2(40, 0);
            boolSetting.text.overflowMode = TextOverflowModes.Ellipsis;

            GameObject nameText = gameObject.transform.Find("NameText").gameObject;
            LocalizedTextMeshProUGUI localizedText = ConfigureLocalizedText(nameText);

            TextMeshProUGUI text = nameText.GetComponent<TextMeshProUGUI>();
            text.text = "Default Text";
            text.richText = true;

            List<Component> externalComponents = gameObject.AddComponent<ExternalComponents>().components;
            externalComponents.Add(text);
            externalComponents.Add(localizedText);

            gameObject.GetComponent<LayoutElement>().preferredWidth = 90;

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
