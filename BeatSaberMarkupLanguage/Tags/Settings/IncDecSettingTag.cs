using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BGLib.Polyglot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public abstract class IncDecSettingTag<T> : BSMLTag
        where T : IncDecSetting
    {
        private FormattedFloatListSettingsController valueControllerTemplate;

        public override GameObject CreateObject(Transform parent)
        {
            if (valueControllerTemplate == null)
            {
                valueControllerTemplate = DiContainer.Resolve<MainSettingsMenuViewController>()._settingsSubMenuInfos.Select(m => m.viewController).First(vc => vc.name == "GraphicSettings").transform.Find("ViewPort/Content/VRRenderingScale").GetComponent<FormattedFloatListSettingsController>();
            }

            FormattedFloatListSettingsController baseSetting = Object.Instantiate(valueControllerTemplate, parent, false);
            baseSetting.name = "BSMLIncDecSetting";

            GameObject gameObject = baseSetting.gameObject;
            Object.Destroy(baseSetting);
            gameObject.SetActive(false);

            T boolSetting = gameObject.AddComponent<T>();
            boolSetting.TextMesh = gameObject.transform.GetChild(1).GetComponentsInChildren<TextMeshProUGUI>().First();
            boolSetting.TextMesh.richText = true;
            boolSetting.DecButton = gameObject.transform.GetChild(1).GetComponentsInChildren<Button>().First();
            boolSetting.IncButton = gameObject.transform.GetChild(1).GetComponentsInChildren<Button>().Last();
            (gameObject.transform.GetChild(1) as RectTransform).sizeDelta = new Vector2(40, 0);
            boolSetting.TextMesh.overflowMode = TextOverflowModes.Ellipsis;

            GameObject nameText = gameObject.transform.Find("NameText").gameObject;
            LocalizedTextMeshProUGUI localizedText = ConfigureLocalizedText(nameText);

            TextMeshProUGUI text = nameText.GetComponent<TextMeshProUGUI>();
            text.text = "Default Text";
            text.richText = true;

            List<Component> externalComponents = gameObject.AddComponent<ExternalComponents>().Components;
            externalComponents.Add(text);
            externalComponents.Add(localizedText);

            gameObject.GetComponent<LayoutElement>().preferredWidth = 90;

            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
