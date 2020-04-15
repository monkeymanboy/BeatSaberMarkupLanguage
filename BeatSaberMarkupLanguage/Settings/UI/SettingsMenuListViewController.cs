using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using System;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Settings.UI.ViewControllers
{
    [ViewDefinition("BeatSaberMarkupLanguage.Views.settings-list.bsml")]
    internal class SettingsMenuListViewController : BSMLAutomaticViewController
    {
        [UIComponent("list")]
        public CustomListTableData list;

        public Action<ViewController> clickedMenu;

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            base.DidActivate(firstActivation, type);
            if (firstActivation)
            {
                rectTransform.sizeDelta = new Vector2(35, 0);
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
            }
            list.data = BSMLSettings.instance.settingsMenus;
            list.tableView?.ReloadData();
        }

        [UIAction("settings-click")]
        private void SettingsClick(TableView tableView, int index)
        {
            clickedMenu?.Invoke((list.data[index] as SettingsMenu).viewController);
        }
    }
}
