using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using UnityEngine;

#if GAME_VERSION_1_29_0
using BeatSaberMarkupLanguage.Util;
#endif

namespace BeatSaberMarkupLanguage.Settings.UI.ViewControllers
{
    [ViewDefinition("BeatSaberMarkupLanguage.Views.settings-list.bsml")]
    internal class SettingsMenuListViewController : BSMLAutomaticViewController
    {
        [UIComponent("list")]
        public CustomListTableData list;

        public Action<SettingsMenu> clickedMenu;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

            if (firstActivation)
            {
                rectTransform.sizeDelta = new Vector2(35, 0);
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
            }

#if GAME_VERSION_1_29_0
            list.data = (SortedList<CustomListTableData.CustomCellInfo>)BSMLSettings.instance.settingsMenus;
#else
            list.data = BSMLSettings.instance.settingsMenus;
#endif

            if (list.tableView != null)
            {
                list.tableView.ReloadData();
            }
        }

        [UIAction("settings-click")]
        private void SettingsClick(TableView tableView, int index)
        {
            SettingsMenu settingsMenu = (SettingsMenu)list.data[index];
            clickedMenu?.Invoke(settingsMenu);
        }
    }
}
