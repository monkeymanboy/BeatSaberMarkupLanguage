using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRUI;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace BeatSaberMarkupLanguage.Settings
{
    public class BSMLSettings : PersistentSingleton<BSMLSettings>
    {
        public List<CustomCellInfo> settingsMenus = new List<CustomCellInfo>();

        public void AddSettingsMenu(string name, string resource, object host)
        {
            //todo if settings menu is empty add button to main screen
            VRUIViewController viewController = BeatSaberUI.CreateViewController<VRUIViewController>();
            viewController.rectTransform.sizeDelta = new Vector2(110, 0);
            viewController.rectTransform.anchorMin = new Vector2(0.5f, 0);
            viewController.rectTransform.anchorMax = new Vector2(0.5f, 1);
            settingsMenus.Add(new SettingsMenu(name, viewController, BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetCallingAssembly(), resource), viewController.gameObject, host)));
        }
    }
}
